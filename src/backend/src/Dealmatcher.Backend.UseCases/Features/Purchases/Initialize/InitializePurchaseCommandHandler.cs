namespace Dealmatcher.Backend.UseCases.Features.Purchases.Initialize;

public sealed class InitializePurchaseCommandHandler(
    IReadRepository<User> userRepository,
    IRepository<Offer> offerRepository,
    IRepository<Purchase> purchaseRepository,
    IDeliveryProviderService deliveryProviderService,
    IPaymentProviderService paymentProviderService,
    IPublisher publisher) : ICommandHandler<InitializePurchaseCommand, Result<InitializePurchaseResult>>
{
    private const string Currency = "PLN";

    public async Task<Result<InitializePurchaseResult>> Handle(InitializePurchaseCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity < 1)
        {
            return Result.Invalid(new ValidationError("Quantity must be at least 1"));
        }

        var buyerSpec = new ActiveUserByIdSpec(request.UserId);
        var buyer = await userRepository.FirstOrDefaultAsync(buyerSpec, cancellationToken);
        if (buyer is null)
        {
            return Result.Unauthorized();
        }

        var offer = await offerRepository.GetByIdAsync(request.OfferId, cancellationToken);
        if (offer is null)
        {
            return Result.NotFound();
        }

        if (offer.Seller.Id == buyer.Id)
        {
            return Result.Conflict("Cannot purchase your own offer");
        }

        if (!offer.Status.CanBeSold)
        {
            return Result.Conflict("Offer is not available for purchase");
        }

        if (offer.Availability < request.Quantity)
        {
            return Result.Conflict("Not enough availability for the requested quantity");
        }

        IDeliveryProvider deliveryProvider;
        try
        {
            deliveryProvider = deliveryProviderService.GetDeliveryProviderById(request.DeliveryMethodId);
        }
        catch (ArgumentException)
        {
            return Result.Invalid(new ValidationError($"Invalid delivery method: {request.DeliveryMethodId}"));
        }

        IPaymentProvider paymentProvider;
        try
        {
            paymentProvider = paymentProviderService.GetPaymentProviderById(request.PaymentMethodId);
        }
        catch (ArgumentException)
        {
            return Result.Invalid(new ValidationError($"Invalid payment method: {request.PaymentMethodId}"));
        }

        var totalPrice = (offer.Price * request.Quantity) + deliveryProvider.Price;

        // Reserve inventory and persist the purchase first. The Availability concurrency
        // token makes this a first-writer-wins operation: a concurrent edit/purchase that
        // already moved Availability causes a ConcurrencyException -> abort (409).
        offer.ReserveQuantity(request.Quantity);

        var purchase = new Purchase(
            buyer,
            offer,
            request.Quantity,
            totalPrice,
            deliveryProvider.Id,
            paymentProvider.Id);

        await purchaseRepository.AddAsync(purchase, cancellationToken);

        try
        {
            await purchaseRepository.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Conflict("Offer was modified concurrently and cannot be purchased");
        }

        // Hook the pending purchase into the expiration background service (15 min TTL).
        await publisher.Publish(new PurchaseCreatedEvent(purchase.Id), cancellationToken);

        // Only now call the external payment provider; the reservation is already committed.
        var session = await paymentProvider.CreatePaymentSessionAsync(totalPrice, Currency);
        purchase.SetPaymentSession(session);
        await purchaseRepository.SaveChangesAsync(cancellationToken);

        var separator = session.RedirectUrl.Contains('?') ? '&' : '?';
        var redirectUrl = $"{session.RedirectUrl}{separator}orderId={purchase.Id}";

        return Result.Success(new InitializePurchaseResult(redirectUrl));
    }
}

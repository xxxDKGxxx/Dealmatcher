namespace Dealmatcher.Backend.UseCases.Features.Purchases.ProcessPayment;

public sealed class ProcessPaymentCommandHandler(
    IRepository<Purchase> purchaseRepository,
    IRepository<Offer> offerRepository,
    IPaymentProviderService paymentProviderService,
    ICartRepository cartRepository,
    ILogger<ProcessPaymentCommandHandler> logger)
    : ICommandHandler<ProcessPaymentCommand, Result>
{
    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken ct)
    {
        var spec = new PurchaseBySessionIdSpec(request.SessionId);
        var purchase = await purchaseRepository.FirstOrDefaultAsync(spec, ct);
        if (purchase is null)
            return Result.NotFound();

        if (purchase.Status.IsFinished)
            return Result.Success();

        IPaymentProvider provider;
        try
        {
            provider = paymentProviderService.GetPaymentProviderById(purchase.PaymentProviderId);
        }
        catch (ArgumentException)
        {
            return Result.Error($"Unknown provider: {purchase.PaymentProviderId}");
        }

        var paymentStatus = provider.ParseStatus(request.ProviderStatus);
        if (paymentStatus is null)
            return Result.Invalid(new ValidationError($"Cannot parse provider status: {request.ProviderStatus}"));

        if (paymentStatus == PaymentStatus.Completed)
        {
            var cart = await cartRepository.GetCartAsync(purchase.Buyer.Id, ct);

            if (cart is not null)
            {
                cart.RemoveItem(purchase.Offer.Id);
                await cartRepository.SaveCartAsync(cart, ct);
            }
            purchase.Complete();
            logger.LogInformation("Purchase {PurchaseId} completed via webhook.", purchase.Id);
        }
        else if (paymentStatus == PaymentStatus.Failed)
        {
            purchase.Fail();
            var offer = await offerRepository.GetByIdAsync(purchase.Offer.Id, ct);
            offer?.RestoreQuantity(purchase.Quantity);
            logger.LogInformation("Purchase {PurchaseId} failed via webhook.", purchase.Id);
        }
        else
        {
            return Result.Success();
        }

        try
        {
            await purchaseRepository.SaveChangesAsync(ct);
        }
        catch (ConcurrencyException)
        {
            return Result.Conflict("Purchase was modified concurrently.");
        }

        return Result.Success();
    }
}

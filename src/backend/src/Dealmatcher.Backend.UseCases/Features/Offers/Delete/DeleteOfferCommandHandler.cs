namespace Dealmatcher.Backend.UseCases.Features.Offers.Delete;

public sealed class DeleteOfferCommandHandler(
    IRepository<Offer> offerRepository,
    IRepository<User> userRepository,
    IReadRepository<Purchase> purchaseRepository) : ICommandHandler<DeleteOfferCommand, Result>
{
    public async Task<Result> Handle(DeleteOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await offerRepository.GetByIdAsync(request.OfferId, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound();
        }

        bool isOwner = offer.Seller.Id == request.UserId;

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        bool isAdmin = user is not null && user.IsPrivileged;
        if (!isOwner && !isAdmin)
        {
            return Result.Forbidden();
        }

        var pendingPurchasesSpec = new PendingPurchasesByOfferIdSpec(request.OfferId);
        var pendingPurchases = await purchaseRepository.ListAsync(pendingPurchasesSpec, cancellationToken);
        if (pendingPurchases.Count > 0)
        {
            return Result.Conflict("Cannot delete offer with active pending purchases");
        }

        try
        {
            await offerRepository.DeleteAsync(offer, cancellationToken);
            await offerRepository.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Conflict("Offer was modified concurrently and cannot be deleted");
        }

        return Result.Success();
    }
}

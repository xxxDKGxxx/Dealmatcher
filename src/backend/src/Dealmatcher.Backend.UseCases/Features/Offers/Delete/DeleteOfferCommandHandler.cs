namespace Dealmatcher.Backend.UseCases.Features.Offers.Delete;

public sealed class DeleteOfferCommandHandler(
    IRepository<Offer> offerRepository) : ICommandHandler<DeleteOfferCommand, Result>
{
    public async Task<Result> Handle(DeleteOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await offerRepository.GetByIdAsync(request.OfferId, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound();
        }

        if (offer.Seller.Id != request.UserId)
        {
            return Result.Forbidden();
        }

        await offerRepository.DeleteAsync(offer, cancellationToken);
        await offerRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

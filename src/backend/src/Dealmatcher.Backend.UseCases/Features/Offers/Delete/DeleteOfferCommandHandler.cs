namespace Dealmatcher.Backend.UseCases.Features.Offers.Delete;

public sealed class DeleteOfferCommandHandler(
    IRepository<Offer> offerRepository,
    IRepository<User> userRepository) : ICommandHandler<DeleteOfferCommand, Result>
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

        await offerRepository.DeleteAsync(offer, cancellationToken);
        await offerRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

namespace Dealmatcher.Backend.UseCases.Features.Offers.ChangeStatus;

public sealed class SetOfferSoldCommandHandler(
    IReadRepository<User> usersRepository,
    IRepository<Offer> offersRepository,
    IMapper mapper) : ICommandHandler<SetOfferSoldCommand, Result<OfferDto>>
{
    public async Task<Result<OfferDto>> Handle(SetOfferSoldCommand request, CancellationToken cancellationToken)
    {
        var activeUserByIdSpec = new ActiveUserByIdSpec(request.userId);
        var user = await usersRepository.FirstOrDefaultAsync(activeUserByIdSpec, cancellationToken);

        if (user is null)
        {
            return Result.Unauthorized($"User with id: {request.userId} doesn't exist");
        }

        var offer = await offersRepository.GetByIdAsync(request.offerId, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound($"Offer with id: {request.offerId} doesn't exist");
        }

        if (user.Id != offer.Seller.Id)
        {
            return Result.Forbidden($"Offer with id: {request.offerId} doesn't belong to user with id: {request.userId}");
        }

        if (!offer.Status.CanBeSold)
        {
            return Result.Conflict($"Cannot set offer status to SOLD from status: {offer.Status}");
        }

        offer.Sell();
        await offersRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(mapper.Map<OfferDto>(offer));
    }
}

namespace Dealmatcher.Backend.UseCases.Features.Offers.ChangeStatus;

public sealed class ActivateOfferCommandHandler(
    IReadRepository<User> usersRepository,
    IRepository<Offer> offersRepository,
    IMapper mapper) : ICommandHandler<ActivateOfferCommand, Result<OfferDto>>
{
    public async Task<Result<OfferDto>> Handle(ActivateOfferCommand request, CancellationToken cancellationToken)
    {
        var activeUserByIdSpec = new ActiveUserByIdSpec(request.adminId);
        var admin = await usersRepository.FirstOrDefaultAsync(activeUserByIdSpec, cancellationToken);

        if (admin is null)
        {
            return Result.Unauthorized($"Admin with id: {request.adminId} not found");
        }

        if (!admin.IsPrivileged)
        {
            return Result.Forbidden($"User with id: {request.adminId} is not an admin");
        }

        var offerByIdNoFiltersSpec = new OfferByIdNoFiltersSpec(request.offerId);
        var offer = await offersRepository.FirstOrDefaultAsync(offerByIdNoFiltersSpec, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound($"Offer with id: {request.offerId} not found");
        }

        if (!offer.Status.CanBeActivated)
        {
            return Result.Conflict($"Offer with status: {offer.Status} cannot be activated");
        }

        if (offer.Seller.Status != UserStatus.Active)
        {
            return Result.Conflict($"Offer from seller in status: {offer.Seller.Status} cannot be activated");
        }

        offer.Activate();

        try
        {
            await offersRepository.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Conflict("Offer was modified concurrently and cannot be activated");
        }

        return Result.Success(mapper.Map<OfferDto>(offer));
    }
}

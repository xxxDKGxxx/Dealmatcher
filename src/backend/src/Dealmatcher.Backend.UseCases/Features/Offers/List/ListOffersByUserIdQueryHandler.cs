using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

namespace Dealmatcher.Backend.UseCases.Features.Offers.List;

public sealed class ListOffersByUserIdQueryHandler(
    IReadRepository<Offer> offersRepository,
    IReadRepository<User> usersRepository,
    IMapper mapper)
    : IQueryHandler<ListOffersByUserIdQuery, Result<List<OfferDto>>>
{
    public async Task<Result<List<OfferDto>>> Handle(ListOffersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound($"User with id {request.UserId} not found");
        }

        var spec = new OffersByUserIdSpec(user.Id);
        var offers = await offersRepository.ListAsync(spec, cancellationToken);

        return Result.Success(mapper.Map<List<OfferDto>>(offers));
    }
}

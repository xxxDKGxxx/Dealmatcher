namespace Dealmatcher.Backend.UseCases.Features.Activities.GetOfferActivities;

public sealed class GetOfferActivityQueryHandler(
    IReadRepository<User> usersRepository,
    IReadRepository<Activity> activitiesRepository,
    IReadRepository<Offer> offersRepository,
    IMapper mapper) : IQueryHandler<GetOfferActivityQuery, Result<List<ActivityDto>>>
{
    public async Task<Result<List<ActivityDto>>> Handle(GetOfferActivityQuery request, CancellationToken cancellationToken)
    {
        var admin = await usersRepository.GetByIdAsync(request.AdminId, cancellationToken);

        if (admin is null)
        {
            return Result.Unauthorized($"Admin with id: {request.AdminId} doesn't exist");
        }

        if (!admin.IsPrivileged)
        {
            return Result.Forbidden($"User id: {request.AdminId} isn't privileged");
        }

        var offer = await offersRepository.GetByIdAsync(request.OfferId, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound($"Offer with id: {request.OfferId} not found");
        }

        var activitiesByUserIdFromToSpec = new ActivitiesByOfferIdFromToSpec(request.OfferId, request.From, request.To);
        var activities = await activitiesRepository.ListAsync(activitiesByUserIdFromToSpec, cancellationToken);

        return Result.Success(activities.Select(mapper.Map<ActivityDto>).ToList());
    }
}

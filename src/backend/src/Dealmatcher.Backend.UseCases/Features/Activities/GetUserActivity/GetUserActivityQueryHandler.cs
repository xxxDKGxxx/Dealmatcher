namespace Dealmatcher.Backend.UseCases.Features.Activities.GetUserActivity;

public sealed class GetUserActivityQueryHandler(
    IReadRepository<User> usersRepository,
    IReadRepository<Activity> activitiesRepository,
    IMapper mapper) : IQueryHandler<GetUserActivityQuery, Result<List<ActivityDto>>>
{
    public async Task<Result<List<ActivityDto>>> Handle(GetUserActivityQuery request, CancellationToken cancellationToken)
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

        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound($"User with id: {request.UserId} not found");
        }

        var activitiesByUserIdFromToSpec = new ActivitiesByUserIdFromToSpec(request.UserId, request.From, request.To);
        var activities = await activitiesRepository.ListAsync(activitiesByUserIdFromToSpec, cancellationToken);

        return Result.Success(activities.Select(mapper.Map<ActivityDto>).ToList());
    }
}

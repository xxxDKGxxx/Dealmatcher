namespace Dealmatcher.Backend.UseCases.Features.Activities;

public sealed class ActivityLoggingBehavior<TRequest, TResult>(
    IRepository<Activity> activitiesRepository,
    IHttpContextAccessor httpContextAccessor,
    IReadRepository<User> usersRepository,
    IReadRepository<Offer> offerRepository,
    ILogger<ActivityLoggingBehavior<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : IRequest<TResult>
    where TResult : Ardalis.Result.IResult
{
    public async Task<TResult> Handle(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken ct)
    {
        var result = await next(ct);

        if (request is not ILoggableActivity<TResult> loggable)
            return result;

        if (result.Status != ResultStatus.Ok)
            return result;

        try
        {
            await LogActivity(
                loggable.Action,
                loggable.GetUserId(result),
                loggable.GetOfferId(result),
                loggable.GetDetails(result),
                ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to log activity for {RequestType}.", typeof(TRequest).Name);
        }

        return result;
    }

    private async Task LogActivity(
        ActivityAction action,
        int? userId,
        int? offerId,
        Dictionary<string, string> details,
        CancellationToken ct)
    {
        var ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;
        if (userId is null || ip is null) return;

        var user = await usersRepository.GetByIdAsync(userId.Value, ct);
        if (user is null) return;

        Offer? offer = null;
        if (offerId is not null)
            offer = await offerRepository.GetByIdAsync(offerId.Value, ct);

        var activity = new Activity(user, offer, action, details, ip);
        await activitiesRepository.AddAsync(activity, ct);
        await activitiesRepository.SaveChangesAsync(ct);
    }
}

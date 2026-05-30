namespace Dealmatcher.Backend.UseCases.Features.Activities.GetUserActivity;

public sealed record GetUserActivityQuery(
    int AdminId,
    int UserId,
    DateTime From,
    DateTime To) : IQuery<Result<List<ActivityDto>>>;

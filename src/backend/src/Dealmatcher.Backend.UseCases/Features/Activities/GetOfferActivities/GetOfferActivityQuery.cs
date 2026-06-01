namespace Dealmatcher.Backend.UseCases.Features.Activities.GetOfferActivities;

public sealed record GetOfferActivityQuery(
    int AdminId,
    int OfferId,
    DateTime? From,
    DateTime? To) : IQuery<Result<List<ActivityDto>>>;

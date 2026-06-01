namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate.Specifications;

public sealed class ActivitiesByOfferIdFromToSpec : Specification<Activity>
{
    public ActivitiesByOfferIdFromToSpec(int userId, DateTime from, DateTime to)
    {
        Query.Where(a => a.Offer != null && a.Offer.Id == userId && a.CreatedAt >= from && a.CreatedAt <= to);
    }
}

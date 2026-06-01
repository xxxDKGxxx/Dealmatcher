namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate.Specifications;

public sealed class ActivitiesByOfferIdFromToSpec : Specification<Activity>
{
    public ActivitiesByOfferIdFromToSpec(int offerId, DateTime? from, DateTime? to)
    {
        Query.Where(a => a.Offer != null && a.Offer.Id == offerId);

        if (from is not null)
        {
            Query.Where(a => a.CreatedAt >= from);
        }

        if (to is not null)
        {
            Query.Where(a => a.CreatedAt <= to);
        }
    }
}

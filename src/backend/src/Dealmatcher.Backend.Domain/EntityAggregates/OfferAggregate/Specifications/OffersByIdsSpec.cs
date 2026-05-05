namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

public sealed class OffersByIdsSpec : Specification<Offer>
{
    public OffersByIdsSpec(IEnumerable<int> offerIds)
    {
        Query.Where(o => offerIds.Contains(o.Id));
    }
}

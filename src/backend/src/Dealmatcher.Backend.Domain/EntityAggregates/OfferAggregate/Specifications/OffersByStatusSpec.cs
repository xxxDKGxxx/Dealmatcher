namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

public sealed class OffersByStatusSpec : Specification<Offer>
{
    public OffersByStatusSpec(OfferStatus status)
    {
        Query.IgnoreQueryFilters();
        Query.Where(o => o.Status == status);
    }
}

namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

public sealed class PagedOffersByStatusSpec : Specification<Offer>
{
    public PagedOffersByStatusSpec(int page, int limit, OfferStatus status)
    {
        Query.IgnoreQueryFilters();
        Query.Where(o => o.Status == status)
            .Skip((page - 1) * limit).Take(limit);
    }
}

namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

public sealed class OfferByIdNoFiltersSpec : SingleResultSpecification<Offer>
{
    public OfferByIdNoFiltersSpec(int offerId)
    {
        Query.IgnoreQueryFilters().Where(o => o.Id == offerId);
    }
}

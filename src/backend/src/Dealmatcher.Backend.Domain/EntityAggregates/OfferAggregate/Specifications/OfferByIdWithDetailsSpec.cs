namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

public sealed class OfferByIdWithDetailsSpec : Specification<Offer>
{
    public OfferByIdWithDetailsSpec(int offerId)
    {
        Query
            .Where(o => o.Id == offerId)
            .Include(o => o.Seller)
            .Include(o => o.Category)
            .Include(o => o.Properties)
            .ThenInclude(p => p.PropertyDefinition);
    }
}

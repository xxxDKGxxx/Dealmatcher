namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Specifications;

public class OffersByUserIdSpec : Specification<Offer>
{
    public OffersByUserIdSpec(int userId)
    {
        Query.Where(o => o.Seller.Id == userId);
    }
}

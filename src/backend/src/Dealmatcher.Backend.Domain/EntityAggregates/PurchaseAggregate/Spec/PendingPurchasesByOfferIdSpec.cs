namespace Dealmatcher.Backend.Domain.EntityAggregates.PurchaseAggregate.Spec;

public sealed class PendingPurchasesByOfferIdSpec : Specification<Purchase>
{
    public PendingPurchasesByOfferIdSpec(int offerId)
    {
        Query.Where(p => p.Offer.Id == offerId && p.Status == PurchaseStatus.Pending);
    }
}

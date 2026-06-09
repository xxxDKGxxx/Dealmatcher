namespace Dealmatcher.Backend.Domain.EntityAggregates.PurchaseAggregate.Spec;

public sealed class PendingPurchasesByOfferIdSpec : Specification<Purchase>
{
    public PendingPurchasesByOfferIdSpec(int offerId)
    {
        var pending = PurchaseStatus.Pending;
        Query.Where(p => p.Offer.Id == offerId && p.Status == pending);
    }
}

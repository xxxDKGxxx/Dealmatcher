namespace Dealmatcher.Backend.Domain.EntityAggregates.PurchaseAggregate.Spec;

public sealed class PurchaseBySessionIdAndProviderIdSpec : SingleResultSpecification<Purchase>
{
    public PurchaseBySessionIdAndProviderIdSpec(string sessionId, string paymentProviderId)
    {
        Query.Where(p => p.PaymentSessionId == sessionId && p.PaymentProviderId == paymentProviderId);
    }
}

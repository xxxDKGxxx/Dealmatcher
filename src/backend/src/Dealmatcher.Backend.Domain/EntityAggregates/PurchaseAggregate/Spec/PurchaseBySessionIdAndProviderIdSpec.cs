namespace Dealmatcher.Backend.Domain.EntityAggregates.PurchaseAggregate.Spec;

public sealed class PurchaseBySessionIdSpec : SingleResultSpecification<Purchase>
{
    public PurchaseBySessionIdSpec(string sessionId)
    {
        Query.Where(p => p.PaymentSessionId == sessionId);
    }
}

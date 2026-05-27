namespace Dealmatcher.Backend.Domain.EntityAggregates.PurchaseAggregate;

public abstract class PurchaseStatus(string name, int value) : SmartEnum<PurchaseStatus>(name, value)
{
    public static readonly PurchaseStatus Pending = new PendingStatus();
    public static readonly PurchaseStatus Completed = new CompletedStatus();
    public static readonly PurchaseStatus Failed = new FailedStatus();

    public abstract bool IsFinished { get; }

    private sealed class PendingStatus() : PurchaseStatus("PENDING", 0)
    {
        public override bool IsFinished => false;
    }
    private sealed class CompletedStatus() : PurchaseStatus("COMPLETED", 1)
    {
        public override bool IsFinished => true;
    }
    private sealed class FailedStatus() : PurchaseStatus("FAILED", 2)
    {
        public override bool IsFinished => true;
    }
}

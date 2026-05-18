namespace Dealmatcher.Backend.Domain.Core.Payment;

public abstract class PaymentStatus(string name, int value) : SmartEnum<PaymentStatus>(name, value)
{
    public static readonly PaymentStatus Pending = new PendingStatus();
    public static readonly PaymentStatus Failed = new FailedStatus();
    public static readonly PaymentStatus Completed = new CompletedStatus();

    class PendingStatus() : PaymentStatus("PENDING", 0) { }

    class FailedStatus() : PaymentStatus("FAILED", 1) { }

    class CompletedStatus() : PaymentStatus("COMPLETED", 2) { }
}

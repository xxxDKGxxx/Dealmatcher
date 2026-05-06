namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages;

public abstract class MessageStatus(string name, int value) : SmartEnum<MessageStatus>(name, value)
{
    public static readonly MessageStatus Sent = new SentStatus();
    public static readonly MessageStatus Delivered = new DeliveredStatus();
    public static readonly MessageStatus Read = new ReadStatus();

    public abstract bool WasDelivered { get; }
    public abstract bool WasRead { get; }

    private sealed class SentStatus() : MessageStatus("SENT", 0)
    {
        public override bool WasDelivered => false;
        public override bool WasRead => false;
    }

    private sealed class DeliveredStatus() : MessageStatus("DELIVERED", 1)
    {
        public override bool WasDelivered => true;
        public override bool WasRead => false;
    }

    private sealed class ReadStatus() : MessageStatus("READ", 2)
    {
        public override bool WasDelivered => true;
        public override bool WasRead => true;
    }
}

namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages;

public class MessageStatus(string name, int value) : SmartEnum<MessageStatus>(name, value)
{
    public static readonly MessageStatus Sent = new SentStatus();
    public static readonly MessageStatus Delivered = new DeliveredStatus();
    public static readonly MessageStatus Read = new ReadStatus();

    private sealed class SentStatus() : MessageStatus("SENT", 0) { }

    private sealed class DeliveredStatus() : MessageStatus("DELIVERED", 1) { }

    private sealed class ReadStatus() : MessageStatus("READ", 2) { }
}

namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate;

public class ConversationStatus(string name, int value) : SmartEnum<ConversationStatus>(name, value)
{
    public static readonly ConversationStatus Active = new ActiveStatus();
    public static readonly ConversationStatus Closed = new ClosedStatus();

    private sealed class ActiveStatus() : ConversationStatus("ACTIVE", 0) { }

    private sealed class ClosedStatus() : ConversationStatus("CLOSED", 1) { }
}

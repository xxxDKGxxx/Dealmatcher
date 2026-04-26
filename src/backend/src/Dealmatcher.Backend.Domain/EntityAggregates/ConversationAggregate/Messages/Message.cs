namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages;

public sealed class Message(User sender, string content) : DealmatcherEntityBase
{
    public User Sender { get; private set; } = sender;
    public String Content { get; private set; } = content;
    public MessageStatus Status { get; private set; } = MessageStatus.Delivered;

    public void Read()
    {
        Status = MessageStatus.Read;
    }
}

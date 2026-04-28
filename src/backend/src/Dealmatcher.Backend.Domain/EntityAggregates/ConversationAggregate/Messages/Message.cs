namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages;

public sealed class Message : DealmatcherEntityBase
{
    public User Sender { get; private set; } = null!;
    public String Content { get; private set; } = null!;
    public MessageStatus Status { get; private set; } = MessageStatus.Delivered;

    public Message(User sender, string content)
    {
        Sender = sender;
        Content = content;
    }

    private Message()
    { /* EF */
    }

    public void Read()
    {
        Status = MessageStatus.Read;
    }
}

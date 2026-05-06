namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate;

public class Conversation : DealmatcherEntityBase, IAggregateRoot
{
    public Offer Offer { get; private set; } = null!;
    public User Buyer { get; private set; } = null!;
    public ConversationStatus Status { get; private set; } = ConversationStatus.Active;

    private readonly List<Message> _messages = [];

    public Conversation(Offer offer, User buyer)
    {
        Offer = offer;
        Buyer = buyer;
    }

    private Conversation()
    { /* EF */
    }

    public User Seller => Offer.Seller;
    public Message LastMessage => _messages.OrderByDescending(m => m.CreatedAt).First();

    public int UnreadCount(int readerId) =>
      _messages.Where(m => m.Sender.Id != readerId && m.Status != MessageStatus.Read).Count();

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public void AddMessage(Message message)
    {
        _messages.Add(message);
    }

    public bool HasParticipant(User user)
    {
        return Seller.Id == user.Id || Buyer.Id == user.Id;
    }

    public void ReceiveMessages(User receiver)
    {
        if (!HasParticipant(receiver))
            return;

        foreach (var message in Messages.Where(m => !m.WasSentBy(receiver)).Where(m => !m.Status.WasDelivered))
        {
            message.Receive();
        }
    }

    public void ReadMessages(User reader)
    {
        if (!HasParticipant(reader))
            return;

        foreach (var message in Messages.Where(m => !m.WasSentBy(reader)).Where(m => !m.Status.WasRead))
        {
            message.Read();
        }
    }
}

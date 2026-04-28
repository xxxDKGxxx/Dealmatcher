namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate;

public class Conversation(Offer offer, User buyer) : DealmatcherEntityBase, IAggregateRoot
{
    public Offer Offer { get; private set; } = offer;
    public User Buyer { get; private set; } = buyer;
    public ConversationStatus Status { get; private set; } = ConversationStatus.Active;

    private readonly List<Message> _messages = [];

    public User Seller => Offer.Seller;
    public Message LastMessage => _messages.OrderByDescending(m => m.CreatedAt).First();

    public int UnreadCount(int readerId) =>
      _messages.Where(m => m.Sender.Id != readerId && m.Status != MessageStatus.Read).Count();

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
}

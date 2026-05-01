namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Specifications;

public sealed class ConversationsByUserIdSpec : Specification<Conversation>
{
    public ConversationsByUserIdSpec(int userId)
    {
        Query
            .Where(c => c.Buyer.Id == userId || c.Offer.Seller.Id == userId);
    }
}

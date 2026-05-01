namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Specifications;

public sealed class ConversationsByUserIdSpec : Specification<Conversation>
{
    public ConversationsByUserIdSpec(int userId)
    {
        Query
            .Where(c => c.Buyer.Id == userId || c.Offer.Seller.Id == userId)
            .Include(c => c.Buyer)
            .Include(c => c.Offer).ThenInclude(o => o.Seller)
            .Include(c => c.Offer).ThenInclude(o => o.Category)
            .Include(c => c.Messages);
    }
}

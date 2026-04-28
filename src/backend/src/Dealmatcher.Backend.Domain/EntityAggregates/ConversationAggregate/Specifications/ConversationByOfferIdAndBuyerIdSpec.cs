namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Specifications;

public sealed class ConversationByOfferIdAndBuyerIdSpec : SingleResultSpecification<Conversation>
{
    public ConversationByOfferIdAndBuyerIdSpec(int offerId, int buyerId)
    {
        Query.Where(c => c.Offer.Id == offerId && c.Buyer.Id == buyerId);
    }
}

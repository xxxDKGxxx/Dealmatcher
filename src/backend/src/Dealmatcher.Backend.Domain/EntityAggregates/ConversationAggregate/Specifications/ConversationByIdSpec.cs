namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Specifications;

public sealed class ConversationByIdSpec : SingleResultSpecification<Conversation>
{
    public ConversationByIdSpec(int conversationId)
    {
        Query.Where(c => c.Id == conversationId);
    }
}

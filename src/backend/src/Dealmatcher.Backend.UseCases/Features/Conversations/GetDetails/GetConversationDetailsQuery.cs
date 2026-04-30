namespace Dealmatcher.Backend.UseCases.Features.Conversations.GetDetails;

public sealed record GetConversationDetailsQuery(
    int ConversationId,
    int UserId
) : IQuery<Result<ConversationDetailDto>>;

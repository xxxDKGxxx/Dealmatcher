namespace Dealmatcher.Backend.UseCases.Features.Conversations.GetDetails;

public sealed record GetConversationDetailsQuery(
    int ConversationId,
    int RequestingUserId
) : IQuery<Result<ConversationDetailDto>>;

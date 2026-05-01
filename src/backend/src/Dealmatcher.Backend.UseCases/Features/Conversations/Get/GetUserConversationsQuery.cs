namespace Dealmatcher.Backend.UseCases.Features.Conversations.Get;

public sealed record GetUserConversationsQuery(int UserId) : IQuery<Result<List<ConversationDto>>>;

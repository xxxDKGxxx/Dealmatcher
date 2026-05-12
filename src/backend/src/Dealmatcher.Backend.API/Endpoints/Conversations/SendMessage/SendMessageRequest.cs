namespace Dealmatcher.Backend.API.Endpoints.Conversations.SendMessage;

public sealed record SendMessageRequest(int ConversationId, string Content);

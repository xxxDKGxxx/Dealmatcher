namespace Dealmatcher.Backend.UseCases.Features.Conversations.SendMessage;

public sealed record SendMessageCommand(
    int ConversationId,
    int SenderId,
    string Content) : ICommand<Result<MessageDto>>;

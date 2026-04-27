namespace Dealmatcher.Backend.UseCases.Features.Conversations.Create;

public sealed record CreateConversationCommand(
    int buyerId,
    int offerId,
    string InitialMessage) : ICommand<Result<ConversationDto>>;

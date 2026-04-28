namespace Dealmatcher.Backend.UseCases.Features.Conversations.Create;

public sealed record CreateConversationCommand(
    int BuyerId,
    int OfferId,
    string InitialMessage) : ICommand<Result<ConversationDto>>;

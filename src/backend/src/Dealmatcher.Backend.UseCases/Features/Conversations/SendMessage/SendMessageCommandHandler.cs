namespace Dealmatcher.Backend.UseCases.Features.Conversations.SendMessage;

public sealed class SendMessageCommandHandler(
    IRepository<Conversation> conversationRepository,
    IReadRepository<User> userRepository,
    IMapper mapper) : ICommandHandler<SendMessageCommand, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result.Invalid(new ValidationError("Message content cannot be empty."));
        }

        var activeUserSpec = new ActiveUserByIdSpec(request.SenderId);
        var sender = await userRepository.FirstOrDefaultAsync(activeUserSpec, cancellationToken);
        if (sender is null)
        {
            return Result.Invalid(new ValidationError($"Invalid Sender Id: {request.SenderId}"));
        }

        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);

        if (conversation is null)
        {
            return Result.NotFound($"Conversation with id {request.ConversationId} was not found.");
        }

        if (!conversation.HasParticipant(sender))
        {
            return Result.Forbidden("You are not a participant in this conversation.");
        }

        if (conversation.Status == ConversationStatus.Closed)
        {
            return Result.Conflict("Cannot send messages to a closed conversation.");
        }

        var message = new Message(sender, request.Content);
        conversation.AddMessage(message);

        await conversationRepository.SaveChangesAsync(cancellationToken);

        var messageDto = mapper.Map<MessageDto>(message);
        return Result.Success(messageDto);
    }
}

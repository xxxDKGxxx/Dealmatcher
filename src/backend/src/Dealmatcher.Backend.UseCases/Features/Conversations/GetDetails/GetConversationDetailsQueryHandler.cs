namespace Dealmatcher.Backend.UseCases.Features.Conversations.GetDetails;

public sealed class GetConversationDetailsQueryHandler(
    IReadRepository<Conversation> conversationsRepository,
    IReadRepository<User> usersRepository,
    IMapper mapper) : IQueryHandler<GetConversationDetailsQuery, Result<ConversationDetailDto>>
{
    public async Task<Result<ConversationDetailDto>> Handle(GetConversationDetailsQuery request, CancellationToken cancellationToken)
    {
        var userByIdSpec = new ActiveUserByIdSpec(request.RequestingUserId);
        var user = await usersRepository.FirstOrDefaultAsync(userByIdSpec, cancellationToken);

        if (user is null)
        {
            return Result.NotFound($"User with id: {request.RequestingUserId} not found");
        }

        var conversation = await conversationsRepository.GetByIdAsync(request.ConversationId);
        if (conversation is null)
        {
            return Result.NotFound($"Conversation with id: {request.ConversationId} not found");
        }

        if (!conversation.HasParticipant(user))
        {
            return Result.Forbidden($"User with id: {request.RequestingUserId} doesn't participate in the conversation");
        }

        return Result.Success(mapper.Map<ConversationDetailDto>(conversation, opts =>
        {
            opts.Items["readerId"] = request.RequestingUserId;
        }));
    }
}

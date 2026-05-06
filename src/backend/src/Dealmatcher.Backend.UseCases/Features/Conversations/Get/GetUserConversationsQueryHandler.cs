namespace Dealmatcher.Backend.UseCases.Features.Conversations.Get;

public sealed class GetUserConversationsQueryHandler(
    IRepository<Conversation> conversationRepository,
    IReadRepository<User> usersRepository,
    IMapper mapper) : IQueryHandler<GetUserConversationsQuery, Result<List<ConversationDto>>>
{
    public async Task<Result<List<ConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        var activeUserByIdSpec = new ActiveUserByIdSpec(request.UserId);
        var user = await usersRepository.FirstOrDefaultAsync(activeUserByIdSpec, cancellationToken);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        var conversationsByUserIdSpec = new ConversationsByUserIdSpec(user.Id);
        var conversations = await conversationRepository.ListAsync(conversationsByUserIdSpec, cancellationToken);

        foreach (var conversation in conversations)
        {
            conversation.ReceiveMessages(user);
        }

        await conversationRepository.SaveChangesAsync(cancellationToken);

        var conversationDtos = mapper.Map<List<ConversationDto>>(conversations, opts =>
        {
            opts.Items["readerId"] = user.Id;
        });

        return Result.Success(conversationDtos);
    }
}

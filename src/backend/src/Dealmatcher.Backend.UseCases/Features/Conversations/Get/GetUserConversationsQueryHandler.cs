namespace Dealmatcher.Backend.UseCases.Features.Conversations.Get;

public sealed class GetUserConversationsQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    IMapper mapper) : IQueryHandler<GetUserConversationsQuery, Result<List<ConversationDto>>>
{
    public async Task<Result<List<ConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationsByUserIdSpec(request.UserId);

        var conversations = await conversationRepository.ListAsync(spec, cancellationToken);

        var conversationDtos = mapper.Map<List<ConversationDto>>(conversations, opts =>
        {
            opts.Items["readerId"] = request.UserId;
        });

        return Result.Success(conversationDtos);
    }
}

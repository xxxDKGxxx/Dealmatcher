namespace Dealmatcher.Backend.UseCases.Features.Conversations.Get;

public sealed class GetUserConversationsQueryHandler(
    IReadRepository<Conversation> conversationRepository,
    IMapper mapper) : IQueryHandler<GetUserConversationsQuery, Result<IEnumerable<ConversationDto>>>
{
    public async Task<Result<IEnumerable<ConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ConversationsByUserIdSpec(request.UserId);

        var conversations = await conversationRepository.ListAsync(spec, cancellationToken);

        if (conversations.Count == 0)
        {
            return Result.Success(Enumerable.Empty<ConversationDto>());
        }

        var conversationDtos = mapper.Map<IEnumerable<ConversationDto>>(conversations, opts =>
        {
            opts.Items["readerId"] = request.UserId;
        });

        return Result.Success(conversationDtos);
    }
}

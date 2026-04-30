namespace Dealmatcher.Backend.UseCases.Features.Conversations.GetDetails;

public sealed class GetConversationDetailsQueryHandler(
    IReadRepository<Conversation> conversationsRepository,
    IReadRepository<User> usersRepository,
    IMapper mapper) : IQueryHandler<GetConversationDetailsQuery, Result<ConversationDetailDto>>
{
    public async Task<Result<ConversationDetailDto>> Handle(GetConversationDetailsQuery request, CancellationToken cancellationToken)
    {
        var userByIdSpec = new ActiveUserByIdSpec(request.UserId);
        var user = await usersRepository.FirstOrDefaultAsync(userByIdSpec, cancellationToken);

        if (user is null)
        {
            return Result.NotFound($"User with id: {request.UserId} not found");
        }

        var conversationByIdSpec = new ConversationByIdSpec(request.ConversationId);
        var conversation = await conversationsRepository.FirstOrDefaultAsync(conversationByIdSpec, cancellationToken);
        if (conversation is null)
        {
            return Result.NotFound($"Conversation with id: {request.ConversationId} not found");
        }

        if (conversation.Buyer.Id != user.Id && conversation.Seller.Id != user.Id)
        {
            return Result.Forbidden($"User with id: {request.UserId} doesn't participate in the conversation");
        }

        return Result.Success(mapper.Map<ConversationDetailDto>(conversation, opts =>
        {
            opts.Items["readerId"] = request.UserId;
        }));
    }
}

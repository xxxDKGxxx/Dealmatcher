namespace Dealmatcher.Backend.UseCases.Features.Conversations.Create;

public sealed class CreateConversationCommandHandler(
    IReadRepository<Offer> offersRepository,
    IReadRepository<User> userRepository,
    IRepository<Conversation> conversationsRepository,
    IMapper mapper) : ICommandHandler<CreateConversationCommand, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.InitialMessage))
        {
            return Result.Invalid(new ValidationError("Initial message must have content"));
        }

        var userByIdSpec = new ActiveUserByIdSpec(request.BuyerId);
        var buyer = await userRepository.FirstOrDefaultAsync(userByIdSpec, cancellationToken);

        if (buyer is null)
        {
            return Result.Invalid(new ValidationError($"Invalid Buyer Id: {request.BuyerId}"));
        }

        var offerByIdSpec = new OfferByIdWithDetailsSpec(request.OfferId);
        var offer = await offersRepository.FirstOrDefaultAsync(offerByIdSpec, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound($"Offer with id {request.OfferId} was not found");
        }

        if (offer.Seller.Id == request.BuyerId)
        {
            return Result.Forbidden("Cannot create a conversation with yourself");
        }

        var conversationByOfferIdAndBuyerIdSpec = new ConversationByOfferIdAndBuyerIdSpec(request.OfferId, request.BuyerId);
        var conversationAlreadyExisting = await conversationsRepository.FirstOrDefaultAsync(conversationByOfferIdAndBuyerIdSpec, cancellationToken);

        if (conversationAlreadyExisting is not null)
        {
            return Result.Conflict($"Conversation for offer {request.OfferId} with buyer {request.BuyerId} already exists");
        }

        var conversation = new Conversation(offer, buyer);
        var message = new Message(buyer, request.InitialMessage);
        conversation.AddMessage(message);
        await conversationsRepository.AddAsync(conversation);
        await conversationsRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(mapper.Map<ConversationDto>(conversation));
    }
}


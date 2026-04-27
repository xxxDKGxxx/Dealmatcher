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

        var userByIdSpec = new ActiveUserByIdSpec(request.buyerId);
        var buyer = await userRepository.FirstOrDefaultAsync(userByIdSpec, cancellationToken);

        if (buyer is null)
        {
            return Result.Invalid(new ValidationError($"Invalid Buyer Id: {request.buyerId}"));
        }

        var offerByIdSpec = new OfferByIdWithDetailsSpec(request.offerId);
        var offer = await offersRepository.FirstOrDefaultAsync(offerByIdSpec, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound($"Offer with id {request.offerId} was not found");
        }

        if (offer.Seller.Id == request.buyerId)
        {
            return Result.Forbidden("Cannot create a conversation with yourself");
        }

        var conversationByOfferIdAndBuyerIdSpec = new ConversationByOfferIdAndBuyerIdSpec(request.offerId, request.buyerId);
        var conversationAlreadyExisting = await conversationsRepository.FirstOrDefaultAsync(conversationByOfferIdAndBuyerIdSpec, cancellationToken);

        if (conversationAlreadyExisting is not null)
        {
            return Result.Conflict($"Conversation for offer {request.offerId} with buyer {request.buyerId} already exists");
        }

        var conversation = new Conversation(offer, buyer);
        await conversationsRepository.AddAsync(conversation);

        return Result.Success(mapper.Map<ConversationDto>(conversation));
    }
}


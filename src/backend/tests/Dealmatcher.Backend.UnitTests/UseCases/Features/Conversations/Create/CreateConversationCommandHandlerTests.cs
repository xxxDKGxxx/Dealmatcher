using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate;
using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Dto;
using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Specifications;
using Dealmatcher.Backend.UseCases.Features.Conversations.Create;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Conversations.Create;

public class CreateConversationCommandHandlerTests
{
    private readonly IReadRepository<Offer> _offerRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IMapper _mapper;
    private readonly CreateConversationCommandHandler _handler;

    public CreateConversationCommandHandlerTests()
    {
        _offerRepository = Substitute.For<IReadRepository<Offer>>();
        _userRepository = Substitute.For<IReadRepository<User>>();
        _conversationRepository = Substitute.For<IRepository<Conversation>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateConversationCommandHandler(
            _offerRepository, _userRepository, _conversationRepository, _mapper);
    }

    private static User CreateUser(int id = 1, string email = "buyer@example.com")
    {
        var user = new User(email, "hash", "Test", "User")
        {
            Id = id
        };
        return user;
    }

    private static Offer CreateOffer(User seller, int id = 10)
    {
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test Offer", "Description", 1000m, [], seller, [], 1, category, [])
        {
            Id = id
        };
        return offer;
    }

    private static CreateConversationCommand CreateValidCommand(int buyerId = 1, int offerId = 10)
    {
        return new CreateConversationCommand(buyerId, offerId, "Hello, is this still available?");
    }

    private void SetupBuyer(User buyer)
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
    }

    private void SetupOffer(Offer offer)
    {
        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .Returns(offer);
    }

    private void SetupNoExistingConversation()
    {
        _conversationRepository.FirstOrDefaultAsync(Arg.Any<ConversationByOfferIdAndBuyerIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Conversation?)null);
    }

    private void SetupExistingConversation(Offer offer, User buyer)
    {
        var conversation = new Conversation(offer, buyer);
        _conversationRepository.FirstOrDefaultAsync(Arg.Any<ConversationByOfferIdAndBuyerIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
    }

    private void SetupMapper()
    {
        _mapper.Map<ConversationDto>(Arg.Any<Conversation>())
            .Returns(callInfo =>
            {
                var conv = callInfo.Arg<Conversation>();
                return new ConversationDto(
                    conv.Id,
                    new OfferDto(conv.Offer.Id, conv.Offer.Title, conv.Offer.Description, conv.Offer.Price,
                        [], new SellerDto(conv.Offer.Seller.Id, conv.Offer.Seller.Name),
                        new CategoryDto(0, "Cars", "Vehicles"), [], [],
                        1, "ACTIVE", DateTime.UtcNow, DateTime.UtcNow),
                    new ConversationParticipantDto(conv.Buyer.Id, conv.Buyer.Name),
                    new ConversationParticipantDto(conv.Offer.Seller.Id, conv.Offer.Seller.Name),
                    conv.LastMessage?.Content ?? "",
                    conv.LastMessage?.CreatedAt ?? DateTime.UtcNow,
                    0,
                    conv.Status.Name,
                    conv.CreatedAt);
            });
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsSuccess()
    {
        var buyer = CreateUser();
        var seller = CreateUser(id: 2, email: "seller@example.com");
        var offer = CreateOffer(seller);

        SetupBuyer(buyer);
        SetupOffer(offer);
        SetupNoExistingConversation();
        SetupMapper();

        var command = CreateValidCommand();
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ValidData_AddsConversationAndSaves()
    {
        var buyer = CreateUser();
        var seller = CreateUser(id: 2, email: "seller@example.com");
        var offer = CreateOffer(seller);

        SetupBuyer(buyer);
        SetupOffer(offer);
        SetupNoExistingConversation();
        SetupMapper();

        var command = CreateValidCommand();
        await _handler.Handle(command, CancellationToken.None);

        await _conversationRepository.Received(1).AddAsync(Arg.Any<Conversation>(), Arg.Any<CancellationToken>());
        await _conversationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidData_ConversationHasInitialMessage()
    {
        var buyer = CreateUser();
        var seller = CreateUser(id: 2, email: "seller@example.com");
        var offer = CreateOffer(seller);

        SetupBuyer(buyer);
        SetupOffer(offer);
        SetupNoExistingConversation();
        SetupMapper();

        var command = CreateValidCommand();
        await _handler.Handle(command, CancellationToken.None);

        await _conversationRepository.Received(1).AddAsync(
            Arg.Is<Conversation>(c => c.Messages.Count == 1
                && c.Messages.First().Content == "Hello, is this still available?"
                && c.Messages.First().Sender.Id == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyMessage_ReturnsInvalid()
    {
        var command = new CreateConversationCommand(1, 10, "");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _conversationRepository.DidNotReceive().AddAsync(Arg.Any<Conversation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NullMessage_ReturnsInvalid()
    {
        var command = new CreateConversationCommand(1, 10, null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _conversationRepository.DidNotReceive().AddAsync(Arg.Any<Conversation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BuyerNotFound_ReturnsInvalid()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = CreateValidCommand();
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _offerRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        var buyer = CreateUser();
        SetupBuyer(buyer);
        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .Returns((Offer?)null);

        var command = CreateValidCommand();
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_BuyerIsSeller_ReturnsForbidden()
    {
        var user = CreateUser(id: 1);
        var offer = CreateOffer(user);

        SetupBuyer(user);
        SetupOffer(offer);

        var command = CreateValidCommand(buyerId: 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _conversationRepository.DidNotReceive().AddAsync(Arg.Any<Conversation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConversationAlreadyExists_ReturnsConflict()
    {
        var buyer = CreateUser();
        var seller = CreateUser(id: 2, email: "seller@example.com");
        var offer = CreateOffer(seller);

        SetupBuyer(buyer);
        SetupOffer(offer);
        SetupExistingConversation(offer, buyer);

        var command = CreateValidCommand();
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _conversationRepository.DidNotReceive().AddAsync(Arg.Any<Conversation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BuyerNotFound_DoesNotCheckOffer()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = CreateValidCommand();
        await _handler.Handle(command, CancellationToken.None);

        await _offerRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_DoesNotCheckExistingConversation()
    {
        var buyer = CreateUser();
        SetupBuyer(buyer);
        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .Returns((Offer?)null);

        var command = CreateValidCommand();
        await _handler.Handle(command, CancellationToken.None);

        await _conversationRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<ConversationByOfferIdAndBuyerIdSpec>(), Arg.Any<CancellationToken>());
    }
}

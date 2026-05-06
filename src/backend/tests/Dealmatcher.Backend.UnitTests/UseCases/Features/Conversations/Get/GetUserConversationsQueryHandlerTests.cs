namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Conversations.Get;

public class GetUserConversationsQueryHandlerTests
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IReadRepository<User> _usersRepository;
    private readonly IMapper _mapper;
    private readonly GetUserConversationsQueryHandler _handler;

    public GetUserConversationsQueryHandlerTests()
    {
        _conversationRepository = Substitute.For<IRepository<Conversation>>();
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetUserConversationsQueryHandler(_conversationRepository, _usersRepository, _mapper);
    }

    private static User CreateUser(int id, string email = "user@example.com")
    {
        var user = new User(email, "hash", "Test", "User");
        typeof(User).GetProperty("Id")?.SetValue(user, id);
        return user;
    }

    private static Offer CreateOffer(User seller, int id = 10)
    {
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test Offer", "Description", 1000m, [], seller, [], 1, category, []);
        typeof(Offer).GetProperty("Id")?.SetValue(offer, id);
        return offer;
    }

    private static Conversation CreateConversation(Offer offer, User buyer, int id = 100)
    {
        var conversation = new Conversation(offer, buyer);
        typeof(Conversation).GetProperty("Id")?.SetValue(conversation, id);
        return conversation;
    }

    private void SetupMapper(List<ConversationDto> dtos)
    {
        _mapper.Map<List<ConversationDto>>(Arg.Any<List<Conversation>>(), Arg.Any<Action<IMappingOperationOptions<object, List<ConversationDto>>>>())
            .Returns(dtos);
    }

    [Fact]
    public async Task Handle_UserHasConversations_ReturnsSuccessWithMappedDtos()
    {
        var userId = 1;
        var buyer = CreateUser(userId, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer, 100);

        var conversationsList = new List<Conversation> { conversation };

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(conversationsList);

        var expectedDto = new ConversationDto(
            Id: 100,
            Offer: null!,
            Buyer: null!,
            Seller: null!,
            LastMessage: "Hello",
            LastMessageAt: DateTime.UtcNow,
            UnreadCount: 0,
            Status: "ACTIVE",
            CreatedAt: DateTime.UtcNow);

        SetupMapper([expectedDto]);

        var query = new GetUserConversationsQuery(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value.First().Id.ShouldBe(100);
        result.Value.First().LastMessage.ShouldBe("Hello");
    }

    [Fact]
    public async Task Handle_UserHasNoConversations_ReturnsSuccessWithEmptyList()
    {
        var userId = 1;
        var user = CreateUser(userId);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Conversation>());
        SetupMapper([]);

        var query = new GetUserConversationsQuery(userId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_ReceivesUndeliveredMessages_StatusChangesToDelivered()
    {
        var buyer = CreateUser(1, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer);

        var sellerMsg = new Message(seller, "Hi there");
        conversation.AddMessage(sellerMsg);

        sellerMsg.Status.ShouldBe(MessageStatus.Sent);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Conversation> { conversation });
        SetupMapper([new ConversationDto(100, null!, null!, null!, "Hi there", DateTime.UtcNow, 0, "ACTIVE", DateTime.UtcNow)]);

        await _handler.Handle(new GetUserConversationsQuery(1), CancellationToken.None);

        sellerMsg.Status.ShouldBe(MessageStatus.Delivered);
    }

    [Fact]
    public async Task Handle_DoesNotReceiveOwnMessages()
    {
        var buyer = CreateUser(1, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer);

        var buyerMsg = new Message(buyer, "Hello");
        conversation.AddMessage(buyerMsg);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Conversation> { conversation });
        SetupMapper([new ConversationDto(100, null!, null!, null!, "Hello", DateTime.UtcNow, 0, "ACTIVE", DateTime.UtcNow)]);

        await _handler.Handle(new GetUserConversationsQuery(1), CancellationToken.None);

        buyerMsg.Status.ShouldBe(MessageStatus.Sent);
    }

    [Fact]
    public async Task Handle_AlreadyDeliveredMessages_StatusUnchanged()
    {
        var buyer = CreateUser(1, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer);

        var sellerMsg = new Message(seller, "Hi");
        sellerMsg.Receive();
        conversation.AddMessage(sellerMsg);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Conversation> { conversation });
        SetupMapper([new ConversationDto(100, null!, null!, null!, "Hi", DateTime.UtcNow, 0, "ACTIVE", DateTime.UtcNow)]);

        await _handler.Handle(new GetUserConversationsQuery(1), CancellationToken.None);

        sellerMsg.Status.ShouldBe(MessageStatus.Delivered);
    }

    [Fact]
    public async Task Handle_SavesChangesAfterReceiving()
    {
        var buyer = CreateUser(1, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer);
        conversation.AddMessage(new Message(seller, "Hi"));

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<Conversation> { conversation });
        SetupMapper([new ConversationDto(100, null!, null!, null!, "Hi", DateTime.UtcNow, 0, "ACTIVE", DateTime.UtcNow)]);

        await _handler.Handle(new GetUserConversationsQuery(1), CancellationToken.None);

        await _conversationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotSaveChanges()
    {
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        await _handler.Handle(new GetUserConversationsQuery(99), CancellationToken.None);

        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

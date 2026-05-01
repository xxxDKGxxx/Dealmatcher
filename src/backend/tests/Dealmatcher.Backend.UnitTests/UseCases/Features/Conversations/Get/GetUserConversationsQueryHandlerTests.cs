namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Conversations.Get;

public class GetUserConversationsQueryHandlerTests
{
    private readonly IReadRepository<Conversation> _conversationRepository;
    private readonly IMapper _mapper;
    private readonly GetUserConversationsQueryHandler _handler;

    public GetUserConversationsQueryHandlerTests()
    {
        _conversationRepository = Substitute.For<IReadRepository<Conversation>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetUserConversationsQueryHandler(_conversationRepository, _mapper);
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

    [Fact]
    public async Task Handle_UserHasConversations_ReturnsSuccessWithMappedDtos()
    {
        // Arrange
        var userId = 1;
        var buyer = CreateUser(userId, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer, 100);

        var conversationsList = new List<Conversation> { conversation };

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

        _mapper.Map<IEnumerable<ConversationDto>>(
            conversationsList,
            Arg.Any<Action<IMappingOperationOptions>>())
            .Returns([expectedDto]);

        var query = new GetUserConversationsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(1);
        result.Value.First().Id.ShouldBe(100);
        result.Value.First().LastMessage.ShouldBe("Hello");
    }

    [Fact]
    public async Task Handle_UserHasNoConversations_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var userId = 1;

        _conversationRepository.ListAsync(Arg.Any<ConversationsByUserIdSpec>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetUserConversationsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }
}

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Conversations.GetDetails;

public class GetConversationDetailsQueryHandlerTests
{
    private readonly IReadRepository<Conversation> _conversationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly GetConversationDetailsQueryHandler _handler;

    public GetConversationDetailsQueryHandlerTests()
    {
        _conversationRepository = Substitute.For<IReadRepository<Conversation>>();
        _userRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetConversationDetailsQueryHandler(_conversationRepository, _userRepository, _mapper);
    }

    private static User CreateUser(int id, string email = "user@example.com")
    {
        var user = new User(email, "hash", "Test", "User")
        {
            Id = id
        };
        return user;
    }

    private static Conversation CreateConversation(User seller, User buyer, int id = 1)
    {
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer)
        {
            Id = id
        };
        conversation.AddMessage(new Message(buyer, "Hello"));
        return conversation;
    }

    [Fact]
    public async Task Handle_ValidBuyer_ReturnsSuccess()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var conversation = CreateConversation(seller, buyer);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        _mapper.Map<ConversationDetailDto>(Arg.Any<Conversation>(), Arg.Any<Action<IMappingOperationOptions>>())
            .Returns(new ConversationDetailDto(1, null!, null!, null!, "Hello", DateTime.UtcNow, 0, "ACTIVE", DateTime.UtcNow, []));

        var query = new GetConversationDetailsQuery(1, 2);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ValidSeller_ReturnsSuccess()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var conversation = CreateConversation(seller, buyer);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(seller);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        _mapper.Map<ConversationDetailDto>(Arg.Any<Conversation>(), Arg.Any<Action<IMappingOperationOptions>>())
            .Returns(new ConversationDetailDto(1, null!, null!, null!, "Hello", DateTime.UtcNow, 0, "ACTIVE", DateTime.UtcNow, []));

        var query = new GetConversationDetailsQuery(1, 1);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var query = new GetConversationDetailsQuery(1, 99);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _conversationRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsNotFound()
    {
        var user = CreateUser(1);
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((Conversation?)null);

        var query = new GetConversationDetailsQuery(99, 1);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_UserNotParticipant_ReturnsForbidden()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var outsider = CreateUser(3, "outsider@example.com");
        var conversation = CreateConversation(seller, buyer);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(outsider);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);

        var query = new GetConversationDetailsQuery(1, 3);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotCheckConversation()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var query = new GetConversationDetailsQuery(1, 99);
        await _handler.Handle(query, CancellationToken.None);

        await _conversationRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConversationNotFound_DoesNotCallMapper()
    {
        var user = CreateUser(1);
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((Conversation?)null);

        var query = new GetConversationDetailsQuery(99, 1);
        await _handler.Handle(query, CancellationToken.None);

        _mapper.DidNotReceive().Map<ConversationDetailDto>(Arg.Any<Conversation>(), Arg.Any<Action<IMappingOperationOptions>>());
    }
}

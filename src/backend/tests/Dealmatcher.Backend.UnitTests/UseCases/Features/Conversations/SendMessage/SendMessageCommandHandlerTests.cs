namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Conversations.SendMessage;

public class SendMessageCommandHandlerTests
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly SendMessageCommandHandler _handler;

    public SendMessageCommandHandlerTests()
    {
        _conversationRepository = Substitute.For<IRepository<Conversation>>();
        _userRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new SendMessageCommandHandler(
            _conversationRepository,
            _userRepository,
            _mapper);
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

    private void SetupMocks(Conversation conversation, User sender)
    {
        _conversationRepository.GetByIdAsync(conversation.Id, Arg.Any<CancellationToken>())
            .Returns(conversation);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(sender);

        _mapper.Map<MessageDto>(Arg.Any<Message>())
            .Returns(new MessageDto(1, sender.Id, "Hello", "SENT", DateTime.UtcNow));
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsSuccessAndSavesMessage()
    {
        // Arrange
        var buyer = CreateUser(1, "buyer@example.com");
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer, 100);

        SetupMocks(conversation, buyer);

        var command = new SendMessageCommand(100, 1, "Hello, is this still available?");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _conversationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        conversation.Messages.Count.ShouldBe(1);
        conversation.Messages.First().Content.ShouldBe("Hello, is this still available?");
        conversation.Messages.First().Sender.Id.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_EmptyMessageContent_ReturnsInvalid()
    {
        // Arrange
        var command = new SendMessageCommand(100, 1, "   ");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Message content cannot be empty"));
        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsNotFound()
    {
        // Arrange
        _conversationRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((Conversation?)null);

        var command = new SendMessageCommand(999, 1, "Hello");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserIsNotParticipant_ReturnsForbidden()
    {
        // Arrange
        var buyer = CreateUser(1);
        var seller = CreateUser(2);
        _ = CreateUser(3);
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer, 100);

        _conversationRepository.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(conversation);

        var command = new SendMessageCommand(100, 3, "Hello");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConversationIsClosed_ReturnsConflict()
    {
        // Arrange
        var buyer = CreateUser(1);
        var seller = CreateUser(2);
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer, 100);

        typeof(Conversation).GetProperty("Status")?.SetValue(conversation, ConversationStatus.Closed);

        _conversationRepository.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(conversation);

        var command = new SendMessageCommand(100, 1, "Hello");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);
        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SenderUserNotFoundInDb_ReturnsInvalid()
    {
        // Arrange
        var buyer = CreateUser(1);
        var seller = CreateUser(2);
        var offer = CreateOffer(seller);
        var conversation = CreateConversation(offer, buyer, 100);

        _conversationRepository.GetByIdAsync(100, Arg.Any<CancellationToken>()).Returns(conversation);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new SendMessageCommand(100, 1, "Hello");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid Sender Id"));
        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Conversations.GetDetails;

public class GetConversationDetailsQueryHandlerTests
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly GetConversationDetailsQueryHandler _handler;

    public GetConversationDetailsQueryHandlerTests()
    {
        _conversationRepository = Substitute.For<IRepository<Conversation>>();
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

    private void SetupMapper()
    {
        _mapper.Map<ConversationDetailDto>(Arg.Any<Conversation>(), Arg.Any<Action<IMappingOperationOptions<object, ConversationDetailDto>>>())
            .Returns(callInfo =>
            {
                var conv = callInfo.ArgAt<Conversation>(0);
                var opts = callInfo.ArgAt<Action<IMappingOperationOptions<object, ConversationDetailDto>>>(1);
                var mockOpts = Substitute.For<IMappingOperationOptions<object, ConversationDetailDto>>();
                var items = new Dictionary<string, object>();
                mockOpts.Items.Returns(items);
                opts.Invoke(mockOpts);
                var readerId = (int)items["readerId"];

                return new ConversationDetailDto(
                    conv.Id, null!, null!, null!,
                    conv.LastMessage?.Content ?? "",
                    conv.LastMessage?.CreatedAt ?? DateTime.UtcNow,
                    conv.UnreadCount(readerId),
                    conv.Status.Name,
                    conv.CreatedAt,
                    []);
            });
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
        SetupMapper();

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
        SetupMapper();

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

        _mapper.DidNotReceive().Map<ConversationDetailDto>(Arg.Any<Conversation>(), Arg.Any<Action<IMappingOperationOptions<object, ConversationDetailDto>>>());
    }

    [Fact]
    public async Task Handle_ValidBuyer_MarksSellerMessagesAsRead()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer) { Id = 1 };

        var sellerMsg1 = new Message(seller, "Hi");
        var sellerMsg2 = new Message(seller, "Interested?");
        var buyerMsg = new Message(buyer, "Hello");
        conversation.AddMessage(sellerMsg1);
        conversation.AddMessage(sellerMsg2);
        conversation.AddMessage(buyerMsg);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 2);
        await _handler.Handle(query, CancellationToken.None);

        sellerMsg1.Status.ShouldBe(MessageStatus.Read);
        sellerMsg2.Status.ShouldBe(MessageStatus.Read);
        buyerMsg.Status.ShouldNotBe(MessageStatus.Read);
    }

    [Fact]
    public async Task Handle_ValidBuyer_SavesChangesAfterReading()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var conversation = CreateConversation(seller, buyer);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 2);
        await _handler.Handle(query, CancellationToken.None);

        await _conversationRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Forbidden_DoesNotSaveChanges()
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
        await _handler.Handle(query, CancellationToken.None);

        await _conversationRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyReadMessages_StatusUnchanged()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer) { Id = 1 };

        var msg = new Message(seller, "Hello");
        msg.Receive();
        msg.Read();
        conversation.AddMessage(msg);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 2);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        msg.Status.ShouldBe(MessageStatus.Read);
        result.Value.UnreadCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_BuyerViewsConversation_UnreadCountReflectsUnreadSellerMessages()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer) { Id = 1 };

        conversation.AddMessage(new Message(buyer, "Hello"));
        conversation.AddMessage(new Message(seller, "Hi"));
        conversation.AddMessage(new Message(seller, "Still interested?"));
        conversation.AddMessage(new Message(seller, "Last chance"));

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 2);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.UnreadCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_SellerViewsConversation_UnreadCountReflectsUnreadBuyerMessages()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer) { Id = 1 };

        conversation.AddMessage(new Message(buyer, "Hello"));
        conversation.AddMessage(new Message(buyer, "Are you there?"));
        conversation.AddMessage(new Message(seller, "Yes"));

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(seller);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 1);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.UnreadCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_MixedReadAndUnread_UnreadCountCorrectAfterReading()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer) { Id = 1 };

        var msg1 = new Message(seller, "First");
        msg1.Receive();
        msg1.Read();
        var msg2 = new Message(seller, "Second");
        msg2.Receive();
        var msg3 = new Message(seller, "Third");

        conversation.AddMessage(msg1);
        conversation.AddMessage(msg2);
        conversation.AddMessage(msg3);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 2);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.UnreadCount.ShouldBe(0);
        msg1.Status.ShouldBe(MessageStatus.Read);
        msg2.Status.ShouldBe(MessageStatus.Read);
        msg3.Status.ShouldBe(MessageStatus.Read);
    }

    [Fact]
    public async Task Handle_OnlyOwnMessages_UnreadCountIsZero()
    {
        var seller = CreateUser(1, "seller@example.com");
        var buyer = CreateUser(2, "buyer@example.com");
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []);
        var conversation = new Conversation(offer, buyer) { Id = 1 };

        conversation.AddMessage(new Message(buyer, "Hello"));
        conversation.AddMessage(new Message(buyer, "Anyone?"));

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(buyer);
        _conversationRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(conversation);
        SetupMapper();

        var query = new GetConversationDetailsQuery(1, 2);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.UnreadCount.ShouldBe(0);
    }
}

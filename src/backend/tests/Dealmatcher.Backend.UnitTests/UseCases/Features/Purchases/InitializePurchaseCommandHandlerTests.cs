namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Purchases;

public class InitializePurchaseCommandHandlerTests
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IRepository<Offer> _offerRepository;
    private readonly IRepository<Purchase> _purchaseRepository;
    private readonly IDeliveryProviderService _deliveryProviderService;
    private readonly IPaymentProviderService _paymentProviderService;
    private readonly IPublisher _publisher;
    private readonly IDeliveryProvider _deliveryProvider;
    private readonly IPaymentProvider _paymentProvider;
    private readonly InitializePurchaseCommandHandler _handler;

    private const string DeliveryMethodId = "courier";
    private const string PaymentMethodId = "pay";

    public InitializePurchaseCommandHandlerTests()
    {
        _userRepository = Substitute.For<IReadRepository<User>>();
        _offerRepository = Substitute.For<IRepository<Offer>>();
        _purchaseRepository = Substitute.For<IRepository<Purchase>>();
        _deliveryProviderService = Substitute.For<IDeliveryProviderService>();
        _paymentProviderService = Substitute.For<IPaymentProviderService>();
        _publisher = Substitute.For<IPublisher>();

        _deliveryProvider = Substitute.For<IDeliveryProvider>();
        _deliveryProvider.Id.Returns(DeliveryMethodId);
        _deliveryProvider.Price.Returns(10m);

        _paymentProvider = Substitute.For<IPaymentProvider>();
        _paymentProvider.Id.Returns(PaymentMethodId);
        _paymentProvider.CreatePaymentSessionAsync(Arg.Any<decimal>(), Arg.Any<string>())
            .Returns(ci => new PaymentSession("Provider", "session-1", "https://pay.example/checkout", (decimal)ci[0], (string)ci[1]));

        _deliveryProviderService.GetDeliveryProviderById(DeliveryMethodId).Returns(_deliveryProvider);
        _paymentProviderService.GetPaymentProviderById(PaymentMethodId).Returns(_paymentProvider);

        _handler = new InitializePurchaseCommandHandler(
            _userRepository,
            _offerRepository,
            _purchaseRepository,
            _deliveryProviderService,
            _paymentProviderService,
            _publisher);
    }

    private static User CreateUser(int id, string email = "buyer@example.com")
    {
        return new User(email, "hash", "Test", "User") { Id = id };
    }

    private static Offer CreateActiveOffer(User seller, int id = 10, decimal price = 100m, int availability = 5)
    {
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", price, [], seller, [], availability, category, []) { Id = id };
        offer.Activate();
        return offer;
    }

    private static InitializePurchaseCommand Command(int userId = 1, int offerId = 10, int quantity = 1) =>
        new(userId, offerId, DeliveryMethodId, PaymentMethodId, quantity);

    [Fact]
    public async Task Handle_ValidRequest_ReservesQuantityCreatesPurchaseAndReturnsRedirectUrl()
    {
        var buyer = CreateUser(1);
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateActiveOffer(seller, price: 100m, availability: 5);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var result = await _handler.Handle(Command(quantity: 2), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.RedirectUrl.ShouldStartWith("https://pay.example/checkout");
        result.Value.RedirectUrl.ShouldContain("orderId=");
        offer.Availability.ShouldBe(3);

        // total = 100 * 2 + 10 delivery = 210
        await _paymentProvider.Received(1).CreatePaymentSessionAsync(210m, "PLN");
        await _purchaseRepository.Received(1).AddAsync(Arg.Any<Purchase>(), Arg.Any<CancellationToken>());
        await _purchaseRepository.Received(2).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<PurchaseCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuantityLessThanOne_ReturnsInvalid()
    {
        var result = await _handler.Handle(Command(quantity: 0), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _userRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BuyerNotFound_ReturnsUnauthorized()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _offerRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        var buyer = CreateUser(1);
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns((Offer?)null);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_BuyerIsSeller_ReturnsConflict()
    {
        var buyer = CreateUser(1);
        var offer = CreateActiveOffer(buyer);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotSellable_ReturnsConflict()
    {
        var buyer = CreateUser(1);
        var seller = CreateUser(2, "seller@example.com");
        var category = new Category("Cars", "Vehicles");
        var draftOffer = new Offer("Test", "Desc", 100m, [], seller, [], 5, category, []) { Id = 10 };

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(draftOffer);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InsufficientAvailability_ReturnsConflict()
    {
        var buyer = CreateUser(1);
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateActiveOffer(seller, availability: 1);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var result = await _handler.Handle(Command(quantity: 2), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidDeliveryMethod_ReturnsInvalid()
    {
        var buyer = CreateUser(1);
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateActiveOffer(seller);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        _deliveryProviderService.GetDeliveryProviderById("unknown").Returns(_ => throw new ArgumentException("bad"));

        var command = new InitializePurchaseCommand(1, 10, "unknown", PaymentMethodId, 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidPaymentMethod_ReturnsInvalid()
    {
        var buyer = CreateUser(1);
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateActiveOffer(seller);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        _paymentProviderService.GetPaymentProviderById("unknown").Returns(_ => throw new ArgumentException("bad"));

        var command = new InitializePurchaseCommand(1, 10, DeliveryMethodId, "unknown", 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConcurrencyOnReservation_ReturnsConflictAndDoesNotCallPaymentProvider()
    {
        var buyer = CreateUser(1);
        var seller = CreateUser(2, "seller@example.com");
        var offer = CreateActiveOffer(seller);

        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(buyer);
        _offerRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        _purchaseRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<int>(new ConcurrencyException()));

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _paymentProvider.DidNotReceive().CreatePaymentSessionAsync(Arg.Any<decimal>(), Arg.Any<string>());
        await _publisher.DidNotReceive().Publish(Arg.Any<PurchaseCreatedEvent>(), Arg.Any<CancellationToken>());
    }
}

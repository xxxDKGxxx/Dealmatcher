namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Purchases;

public class ProcessPaymentCommandHandlerTests
{
    private readonly IRepository<Purchase> _purchaseRepository;
    private readonly IRepository<Offer> _offerRepository;
    private readonly IPaymentProviderService _paymentProviderService;
    private readonly IPaymentProvider _paymentProvider;
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;
    private readonly ProcessPaymentCommandHandler _handler;

    public ProcessPaymentCommandHandlerTests()
    {
        _purchaseRepository = Substitute.For<IRepository<Purchase>>();
        _offerRepository = Substitute.For<IRepository<Offer>>();
        _paymentProviderService = Substitute.For<IPaymentProviderService>();
        _paymentProvider = Substitute.For<IPaymentProvider>();
        _cartRepository = Substitute.For<ICartRepository>();
        _logger = Substitute.For<ILogger<ProcessPaymentCommandHandler>>();

        _paymentProviderService.GetPaymentProviderById(Arg.Any<string>()).Returns(_paymentProvider);

        _handler = new ProcessPaymentCommandHandler(
            _purchaseRepository,
            _offerRepository,
            _paymentProviderService,
            _cartRepository,
            _logger);
    }

    private static User CreateUser(int id = 1)
    {
        return new User("buyer@example.com", "hash", "Test", "User") { Id = id };
    }

    private static Offer CreateActiveOffer(int id = 10, int availability = 5)
    {
        var seller = new User("seller@example.com", "hash", "Seller", "User") { Id = 2 };
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 100m, [], seller, [], availability, category, []) { Id = id };
        offer.Activate();
        return offer;
    }

    private static Purchase CreatePendingPurchase(User buyer, Offer offer, int id = 1, string sessionId = "session-1")
    {
        var purchase = new Purchase(buyer, offer, 1, 110m, "courier", "pay") { Id = id };
        purchase.SetPaymentSession(new PaymentSession("pay", sessionId, "https://pay.example", 110m, "PLN"));
        return purchase;
    }

    [Fact]
    public async Task Handle_Completed_CompletesPurchase()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);
        _paymentProvider.ParseStatus(Arg.Any<string>()).Returns(PaymentStatus.Completed);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"COMPLETED"}"""),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        purchase.Status.ShouldBe(PurchaseStatus.Completed);
        await _purchaseRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Failed_FailsPurchaseAndRestoresQuantity()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer(availability: 3);
        var purchase = CreatePendingPurchase(buyer, offer);

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);
        _offerRepository.GetByIdAsync(offer.Id, Arg.Any<CancellationToken>()).Returns(offer);
        _paymentProvider.ParseStatus(Arg.Any<string>()).Returns(PaymentStatus.Failed);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"FAILED"}"""),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        purchase.Status.ShouldBe(PurchaseStatus.Failed);
        offer.Availability.ShouldBe(4);
        await _purchaseRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PurchaseNotFound_ReturnsNotFound()
    {
        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns((Purchase?)null);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("unknown", "{}"),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_AlreadyCompleted_ReturnsSuccessWithoutChanges()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);
        purchase.Complete();

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"COMPLETED"}"""),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyFailed_ReturnsSuccessWithoutChanges()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);
        purchase.Fail();

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"FAILED"}"""),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnparseableBody_ReturnsInvalid()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);
        _paymentProvider.ParseStatus(Arg.Any<string>()).Returns((PaymentStatus?)null);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", "garbage"),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_PendingStatus_ReturnsSuccessWithoutChanges()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);
        _paymentProvider.ParseStatus(Arg.Any<string>()).Returns(PaymentStatus.Pending);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"PENDING"}"""),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        purchase.Status.ShouldBe(PurchaseStatus.Pending);
        await _purchaseRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ConcurrencyException_ReturnsConflict()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);
        _paymentProvider.ParseStatus(Arg.Any<string>()).Returns(PaymentStatus.Completed);
        _purchaseRepository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException<int>(new ConcurrencyException()));

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"COMPLETED"}"""),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
    }

    [Fact]
    public async Task Handle_Failed_DoesNotRestoreIfOfferNotFound()
    {
        var buyer = CreateUser();
        var offer = CreateActiveOffer();
        var purchase = CreatePendingPurchase(buyer, offer);

        _purchaseRepository.FirstOrDefaultAsync(Arg.Any<PurchaseBySessionIdSpec>(), Arg.Any<CancellationToken>()).Returns(purchase);
        _offerRepository.GetByIdAsync(offer.Id, Arg.Any<CancellationToken>()).Returns((Offer?)null);
        _paymentProvider.ParseStatus(Arg.Any<string>()).Returns(PaymentStatus.Failed);

        var result = await _handler.Handle(
            new ProcessPaymentCommand("session-1", """{"status":"FAILED"}"""),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        purchase.Status.ShouldBe(PurchaseStatus.Failed);
    }
}

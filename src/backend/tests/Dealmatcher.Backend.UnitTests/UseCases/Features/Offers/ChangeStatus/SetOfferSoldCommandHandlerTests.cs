namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.ChangeStatus;

public class SetOfferSoldCommandHandlerTests
{
    private readonly IReadRepository<User> _usersRepository;
    private readonly IRepository<Offer> _offersRepository;
    private readonly IReadRepository<Purchase> _purchaseRepository;
    private readonly IMapper _mapper;
    private readonly SetOfferSoldCommandHandler _handler;

    public SetOfferSoldCommandHandlerTests()
    {
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _offersRepository = Substitute.For<IRepository<Offer>>();
        _purchaseRepository = Substitute.For<IReadRepository<Purchase>>();
        _purchaseRepository.ListAsync(Arg.Any<PendingPurchasesByOfferIdSpec>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _mapper = Substitute.For<IMapper>();
        _handler = new SetOfferSoldCommandHandler(_usersRepository, _offersRepository, _purchaseRepository, _mapper);
    }

    private static User CreateUser(int id, string email = "seller@example.com")
    {
        var user = new User(email, "hash", "Test", "User") { Id = id };
        return user;
    }

    private static Offer CreateActiveOffer(User seller, int id = 10)
    {
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []) { Id = id };
        offer.Activate();
        return offer;
    }

    private void SetupMapper()
    {
        _mapper.Map<OfferDto>(Arg.Any<Offer>())
            .Returns(new OfferDto(10, "Test", "Desc", 1000m, [], null!, null!, [], [], 1, "SOLD", DateTime.UtcNow, DateTime.UtcNow));
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessAndSellsOffer()
    {
        var seller = CreateUser(1);
        var offer = CreateActiveOffer(seller);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(seller);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        SetupMapper();

        var result = await _handler.Handle(new SetOfferSoldCommand(1, 10), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        offer.Status.ShouldBe(OfferStatus.Sold);
        await _offersRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUnauthorized()
    {
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new SetOfferSoldCommand(99, 10), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        var user = CreateUser(1);
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(user);
        _offersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Offer?)null);

        var result = await _handler.Handle(new SetOfferSoldCommand(1, 99), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_UserNotOwner_ReturnsForbidden()
    {
        var seller = CreateUser(1);
        var otherUser = CreateUser(2, "other@example.com");
        var offer = CreateActiveOffer(seller);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(otherUser);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var result = await _handler.Handle(new SetOfferSoldCommand(2, 10), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _offersRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferCannotBeSold_ReturnsConflict()
    {
        var seller = CreateUser(1);
        var offer = CreateActiveOffer(seller);
        offer.Sell();

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(seller);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var result = await _handler.Handle(new SetOfferSoldCommand(1, 10), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _offersRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotCheckOffer()
    {
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        await _handler.Handle(new SetOfferSoldCommand(99, 10), CancellationToken.None);

        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Forbidden_DoesNotSave()
    {
        var seller = CreateUser(1);
        var otherUser = CreateUser(2, "other@example.com");
        var offer = CreateActiveOffer(seller);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(otherUser);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        await _handler.Handle(new SetOfferSoldCommand(2, 10), CancellationToken.None);

        await _offersRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

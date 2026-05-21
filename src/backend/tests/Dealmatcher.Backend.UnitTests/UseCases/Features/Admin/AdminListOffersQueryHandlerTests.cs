namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Admin;

public class AdminListOffersQueryHandlerTests
{
    private readonly IReadRepository<User> _usersRepository;
    private readonly IReadRepository<Offer> _offersRepository;
    private readonly IMapper _mapper;
    private readonly AdminListOffersQueryHandler _handler;

    public AdminListOffersQueryHandlerTests()
    {
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _offersRepository = Substitute.For<IReadRepository<Offer>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new AdminListOffersQueryHandler(_usersRepository, _offersRepository, _mapper);
    }

    private static User CreateAdmin(int id = 1)
    {
        var admin = new User("admin@example.com", "hash", "Admin", "User") { Id = id };
        admin.GrantAdminPrivileges();
        return admin;
    }

    private static User CreateRegularUser(int id = 2)
    {
        var user = new User("user@example.com", "hash", "Regular", "User") { Id = id };
        return user;
    }

    private static Offer CreateOffer(int id, string title = "Test")
    {
        var seller = new User("seller@example.com", "hash", "Seller", "User") { Id = 99 };
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer(title, "Desc", 1000m, [], seller, [], 1, category, []) { Id = id };
        return offer;
    }

    private void SetupMapper()
    {
        _mapper.Map<OfferDto>(Arg.Any<Offer>())
            .Returns(callInfo =>
            {
                var offer = callInfo.Arg<Offer>();
                return new OfferDto(offer.Id, offer.Title, "Desc", 1000m, [], null!, null!, [], [], 1, "DRAFT", DateTime.UtcNow, DateTime.UtcNow);
            });
    }

    [Fact]
    public async Task Handle_ValidAdmin_ReturnsSuccessWithOffers()
    {
        var admin = CreateAdmin();
        var offers = new List<Offer> { CreateOffer(1, "Offer1"), CreateOffer(2, "Offer2") };

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(offers);
        _offersRepository.CountAsync(Arg.Any<OffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(2);
        SetupMapper();

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, 20, "DRAFT"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Total.ShouldBe(2);
        result.Value.Page.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ValidAdmin_TotalPagesCalculatedCorrectly()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _offersRepository.CountAsync(Arg.Any<OffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(45);
        SetupMapper();

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, 20, "DRAFT"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Pages.ShouldBe(3);
        result.Value.Total.ShouldBe(45);
    }

    [Fact]
    public async Task Handle_ExactMultiple_TotalPagesCorrect()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _offersRepository.CountAsync(Arg.Any<OffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(40);
        SetupMapper();

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, 20, "DRAFT"), CancellationToken.None);

        result.Value.Pages.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_PageBeyondTotal_ReturnsEmptyItemsWithCorrectMeta()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _offersRepository.CountAsync(Arg.Any<OffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(5);
        SetupMapper();

        var result = await _handler.Handle(new AdminListOffersQuery(1, 999, 20, "DRAFT"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.Page.ShouldBe(1);
        result.Value.Pages.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new AdminListOffersQuery(99, 1, 20, "DRAFT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_NotAdmin_ReturnsForbidden()
    {
        var user = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new AdminListOffersQuery(2, 1, 20, "DRAFT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_InvalidLimit_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, 0, "DRAFT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_NegativeLimit_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, -5, "DRAFT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_InvalidPage_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListOffersQuery(1, 0, 20, "DRAFT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_InvalidStatus_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, 20, "NONEXISTENT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_AdminNotFound_DoesNotQueryOffers()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        await _handler.Handle(new AdminListOffersQuery(99, 1, 20, "DRAFT"), CancellationToken.None);

        await _offersRepository.DidNotReceive().ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>());
        await _offersRepository.DidNotReceive().CountAsync(Arg.Any<OffersByStatusSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotAdmin_DoesNotQueryOffers()
    {
        var user = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new AdminListOffersQuery(2, 1, 20, "DRAFT"), CancellationToken.None);

        await _offersRepository.DidNotReceive().ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoOffers_ReturnsEmptyList()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.ListAsync(Arg.Any<PagedOffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _offersRepository.CountAsync(Arg.Any<OffersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(0);
        SetupMapper();

        var result = await _handler.Handle(new AdminListOffersQuery(1, 1, 20, "ACTIVE"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.Total.ShouldBe(0);
        result.Value.Pages.ShouldBe(0);
    }
}

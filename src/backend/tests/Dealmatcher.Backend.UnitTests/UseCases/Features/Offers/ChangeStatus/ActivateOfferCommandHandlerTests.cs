namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.ChangeStatus;

public class ActivateOfferCommandHandlerTests
{
    private readonly IReadRepository<User> _usersRepository;
    private readonly IRepository<Offer> _offersRepository;
    private readonly IMapper _mapper;
    private readonly ActivateOfferCommandHandler _handler;

    public ActivateOfferCommandHandlerTests()
    {
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _offersRepository = Substitute.For<IRepository<Offer>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ActivateOfferCommandHandler(_usersRepository, _offersRepository, _mapper);
    }

    private static User CreateAdmin(int id = 1)
    {
        var admin = new User("admin@example.com", "hash", "Admin", "User") { Id = id };
        admin.GrantAdminPrivileges();
        return admin;
    }

    private static User CreateRegularUser(int id = 2)
    {
        var user = new BasicUser("user@example.com", "hash", "Regular", "User") { Id = id };
        return user;
    }

    private static Offer CreateDraftOffer(User seller, int id = 10)
    {
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []) { Id = id };
        return offer;
    }

    private void SetupMapper()
    {
        _mapper.Map<OfferDto>(Arg.Any<Offer>())
            .Returns(new OfferDto(10, "Test", "Desc", 1000m, [], null!, null!, [], [], 1, "ACTIVE", DateTime.UtcNow, DateTime.UtcNow));
    }

    [Fact]
    public async Task Handle_ValidAdmin_ReturnsSuccessAndActivatesOffer()
    {
        var admin = CreateAdmin();
        var seller = CreateRegularUser(3);
        var offer = CreateDraftOffer(seller);

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        SetupMapper();

        var result = await _handler.Handle(new ActivateOfferCommand(1, 10), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        offer.Status.ShouldBe(OfferStatus.Active);
        await _offersRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new ActivateOfferCommand(99, 10), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsForbidden()
    {
        var regularUser = CreateRegularUser();

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(regularUser);

        var result = await _handler.Handle(new ActivateOfferCommand(2, 10), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        var admin = CreateAdmin();
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Offer?)null);

        var result = await _handler.Handle(new ActivateOfferCommand(1, 99), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_OfferCannotBeActivated_ReturnsConflict()
    {
        var admin = CreateAdmin();
        var seller = CreateRegularUser(3);
        var offer = CreateDraftOffer(seller);
        offer.Activate();

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        var result = await _handler.Handle(new ActivateOfferCommand(1, 10), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        await _offersRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotAdmin_DoesNotCheckOffer()
    {
        var regularUser = CreateRegularUser();
        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(regularUser);

        await _handler.Handle(new ActivateOfferCommand(2, 10), CancellationToken.None);

        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Conflict_DoesNotSave()
    {
        var admin = CreateAdmin();
        var seller = CreateRegularUser(3);
        var offer = CreateDraftOffer(seller);
        offer.Activate();

        _usersRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);

        await _handler.Handle(new ActivateOfferCommand(1, 10), CancellationToken.None);

        await _offersRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

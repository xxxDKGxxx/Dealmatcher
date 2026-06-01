namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Activities;

public class GetOfferActivityQueryHandlerTests
{
    private readonly IReadRepository<User> _usersRepository;
    private readonly IReadRepository<Activity> _activitiesRepository;
    private readonly IReadRepository<Offer> _offersRepository;
    private readonly IMapper _mapper;
    private readonly GetOfferActivityQueryHandler _handler;

    public GetOfferActivityQueryHandlerTests()
    {
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _activitiesRepository = Substitute.For<IReadRepository<Activity>>();
        _offersRepository = Substitute.For<IReadRepository<Offer>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetOfferActivityQueryHandler(_usersRepository, _activitiesRepository, _offersRepository, _mapper);
    }

    private static User CreateAdmin(int id = 1)
    {
        var admin = new User("admin@example.com", "hash", "Admin", "User") { Id = id };
        admin.GrantAdminPrivileges();
        return admin;
    }

    private static User CreateRegularUser(int id = 2)
    {
        return new User("user@example.com", "hash", "Regular", "User") { Id = id };
    }

    private static Offer CreateOffer(int id = 10)
    {
        var seller = new User("seller@example.com", "hash", "Seller", "User") { Id = 99 };
        var category = new Category("Cars", "Vehicles");
        var offer = new Offer("Test", "Desc", 1000m, [], seller, [], 1, category, []) { Id = id };
        return offer;
    }

    private static Activity CreateActivity(User user, Offer offer, ActivityAction action)
    {
        return new Activity(user, offer, action, [], System.Net.IPAddress.Parse("127.0.0.1"));
    }

    private void SetupMapper()
    {
        _mapper.Map<ActivityDto>(Arg.Any<Activity>())
            .Returns(callInfo =>
            {
                var a = callInfo.Arg<Activity>();
                return new ActivityDto(a.Id, a.User.Id, a.Offer?.Id, a.Action.Name, a.Details.ToDictionary(), a.IPAddress.ToString(), a.CreatedAt);
            });
    }

    [Fact]
    public async Task Handle_ValidAdmin_ReturnsSuccessWithActivities()
    {
        var admin = CreateAdmin();
        var offer = CreateOffer();
        var user = CreateRegularUser();
        var activities = new List<Activity>
        {
            CreateActivity(user, offer, ActivityAction.View),
            CreateActivity(user, offer, ActivityAction.Purchase)
        };

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByOfferIdFromToSpec>(), Arg.Any<CancellationToken>()).Returns(activities);
        SetupMapper();

        var result = await _handler.Handle(new GetOfferActivityQuery(1, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new GetOfferActivityQuery(99, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_NotAdmin_ReturnsForbidden()
    {
        var user = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new GetOfferActivityQuery(2, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Offer?)null);

        var result = await _handler.Handle(new GetOfferActivityQuery(1, 99, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_NoActivities_ReturnsEmptyList()
    {
        var admin = CreateAdmin();
        var offer = CreateOffer();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByOfferIdFromToSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        SetupMapper();

        var result = await _handler.Handle(new GetOfferActivityQuery(1, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_AdminNotFound_DoesNotCheckOffer()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        await _handler.Handle(new GetOfferActivityQuery(99, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotAdmin_DoesNotCheckOffer()
    {
        var user = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new GetOfferActivityQuery(2, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        await _offersRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_DoesNotQueryActivities()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Offer?)null);

        await _handler.Handle(new GetOfferActivityQuery(1, 99, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        await _activitiesRepository.DidNotReceive().ListAsync(Arg.Any<ActivitiesByOfferIdFromToSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapperCalledForEachActivity()
    {
        var admin = CreateAdmin();
        var offer = CreateOffer();
        var user = CreateRegularUser();
        var activities = new List<Activity>
        {
            CreateActivity(user, offer, ActivityAction.View),
            CreateActivity(user, offer, ActivityAction.Purchase),
            CreateActivity(user, offer, ActivityAction.StatusChange)
        };

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _offersRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(offer);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByOfferIdFromToSpec>(), Arg.Any<CancellationToken>()).Returns(activities);
        SetupMapper();

        await _handler.Handle(new GetOfferActivityQuery(1, 10, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow), CancellationToken.None);

        _mapper.Received(3).Map<ActivityDto>(Arg.Any<Activity>());
    }
}

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Activities;

public class GetUserActivityQueryHandlerTests
{
    private readonly IReadRepository<User> _usersRepository;
    private readonly IReadRepository<Activity> _activitiesRepository;
    private readonly IMapper _mapper;
    private readonly GetUserActivityQueryHandler _handler;

    public GetUserActivityQueryHandlerTests()
    {
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _activitiesRepository = Substitute.For<IReadRepository<Activity>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUserActivityQueryHandler(_usersRepository, _activitiesRepository, _mapper);
    }

    private static User CreateAdmin(int id = 1)
    {
        var admin = new User("admin@example.com", "hash", "Admin", "User") { Id = id };
        admin.GrantAdminPrivileges();
        return admin;
    }

    private static User CreateRegularUser(int id = 2, string email = "user@example.com")
    {
        var user = new User(email, "hash", "Regular", "User") { Id = id };
        return user;
    }

    private static Activity CreateActivity(User user, ActivityAction action, int id = 1)
    {
        var activity = new Activity(
            user,
            null,
            action,
            new Dictionary<string, string> { ["test"] = "value" },
            System.Net.IPAddress.Parse("127.0.0.1"));
        typeof(Activity).GetProperty("Id")?.SetValue(activity, id);
        return activity;
    }

    private void SetupMapper()
    {
        _mapper.Map<ActivityDto>(Arg.Any<Activity>())
            .Returns(callInfo =>
            {
                var a = callInfo.Arg<Activity>();
                return new ActivityDto(
                    a.Id,
                    a.User.Id,
                    a.Offer?.Id,
                    a.Action.Name,
                    a.Details.ToDictionary(),
                    a.IPAddress.ToString(),
                    a.CreatedAt);
            });
    }

    [Fact]
    public async Task Handle_ValidAdmin_ReturnsSuccessWithActivities()
    {
        var admin = CreateAdmin();
        var user = CreateRegularUser();
        var activities = new List<Activity>
        {
            CreateActivity(user, ActivityAction.Login, 1),
            CreateActivity(user, ActivityAction.Create, 2)
        };

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>())
            .Returns(activities);
        SetupMapper();

        var query = new GetUserActivityQuery(1, 2, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var query = new GetUserActivityQuery(99, 2, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_NotAdmin_ReturnsForbidden()
    {
        var regularUser = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(regularUser);

        var query = new GetUserActivityQuery(2, 3, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var query = new GetUserActivityQuery(1, 99, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_NoActivities_ReturnsEmptyList()
    {
        var admin = CreateAdmin();
        var user = CreateRegularUser();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        SetupMapper();

        var query = new GetUserActivityQuery(1, 2, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_AdminNotFound_DoesNotCheckUser()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var query = new GetUserActivityQuery(99, 2, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        await _handler.Handle(query, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotAdmin_DoesNotCheckUser()
    {
        var regularUser = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(regularUser);

        var query = new GetUserActivityQuery(2, 3, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        await _handler.Handle(query, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotQueryActivities()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var query = new GetUserActivityQuery(1, 99, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        await _handler.Handle(query, CancellationToken.None);

        await _activitiesRepository.DidNotReceive().ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ActivitiesReturned_MapperCalledForEach()
    {
        var admin = CreateAdmin();
        var user = CreateRegularUser();
        var activities = new List<Activity>
        {
            CreateActivity(user, ActivityAction.Login, 1),
            CreateActivity(user, ActivityAction.Create, 2),
            CreateActivity(user, ActivityAction.Delete, 3)
        };

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>())
            .Returns(activities);
        SetupMapper();

        var query = new GetUserActivityQuery(1, 2, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        await _handler.Handle(query, CancellationToken.None);

        _mapper.Received(3).Map<ActivityDto>(Arg.Any<Activity>());
    }

    [Fact]
    public async Task Handle_CorrectDtosMapped()
    {
        var admin = CreateAdmin();
        var user = CreateRegularUser();
        var activity = CreateActivity(user, ActivityAction.Login, 5);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);
        _activitiesRepository.ListAsync(Arg.Any<ActivitiesByUserIdFromToSpec>(), Arg.Any<CancellationToken>()).Returns([activity]);
        SetupMapper();

        var query = new GetUserActivityQuery(1, 2, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Value.Count.ShouldBe(1);
        result.Value.First().UserId.ShouldBe(2);
        result.Value.First().Action.ShouldBe("LOGIN");
    }
}

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Admin;

public class AdminListUsersQueryHandlerTests
{
    private readonly IReadRepository<User> _usersRepository;
    private readonly IMapper _mapper;
    private readonly AdminListUsersQueryHandler _handler;

    public AdminListUsersQueryHandlerTests()
    {
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new AdminListUsersQueryHandler(_usersRepository, _mapper);
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

    private void SetupMapper()
    {
        _mapper.Map<UserDto>(Arg.Any<User>())
            .Returns(callInfo =>
            {
                var user = callInfo.Arg<User>();
                return new UserDto(user.Id, user.Email, user.Name, user.Surname, user.Status.Name, user.CreatedAt);
            });
    }

    [Fact]
    public async Task Handle_ValidAdmin_ReturnsSuccessWithUsers()
    {
        var admin = CreateAdmin();
        var users = new List<User> { CreateRegularUser(2, "user1@example.com"), CreateRegularUser(3, "user2@example.com") };

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(users);
        _usersRepository.CountAsync(Arg.Any<UsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(2);
        SetupMapper();

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, 20, "ACTIVE"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(2);
        result.Value.Total.ShouldBe(2);
        result.Value.Page.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_TotalPagesCalculatedCorrectly()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _usersRepository.CountAsync(Arg.Any<UsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(45);
        SetupMapper();

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, 20, "ACTIVE"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Pages.ShouldBe(3);
        result.Value.Total.ShouldBe(45);
    }

    [Fact]
    public async Task Handle_ExactMultiple_TotalPagesCorrect()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _usersRepository.CountAsync(Arg.Any<UsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(40);
        SetupMapper();

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, 20, "ACTIVE"), CancellationToken.None);

        result.Value.Pages.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_PageBeyondTotal_ReturnsEmptyItemsWithCorrectMeta()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _usersRepository.CountAsync(Arg.Any<UsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(5);
        SetupMapper();

        var result = await _handler.Handle(new AdminListUsersQuery(1, 999, 20, "ACTIVE"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.Page.ShouldBe(1);
        result.Value.Pages.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new AdminListUsersQuery(99, 1, 20, "ACTIVE"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_NotAdmin_ReturnsForbidden()
    {
        var user = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _handler.Handle(new AdminListUsersQuery(2, 1, 20, "ACTIVE"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_InvalidLimit_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, 0, "ACTIVE"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_NegativeLimit_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, -5, "ACTIVE"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_InvalidPage_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListUsersQuery(1, 0, 20, "ACTIVE"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_InvalidStatus_ReturnsInvalid()
    {
        var admin = CreateAdmin();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, 20, "NONEXISTENT"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_AdminNotFound_DoesNotQueryUsers()
    {
        _usersRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        await _handler.Handle(new AdminListUsersQuery(99, 1, 20, "ACTIVE"), CancellationToken.None);

        await _usersRepository.DidNotReceive().ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>());
        await _usersRepository.DidNotReceive().CountAsync(Arg.Any<UsersByStatusSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotAdmin_DoesNotQueryUsers()
    {
        var user = CreateRegularUser();
        _usersRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(user);

        await _handler.Handle(new AdminListUsersQuery(2, 1, 20, "ACTIVE"), CancellationToken.None);

        await _usersRepository.DidNotReceive().ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoUsers_ReturnsEmptyList()
    {
        var admin = CreateAdmin();

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _usersRepository.ListAsync(Arg.Any<PagedUsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns([]);
        _usersRepository.CountAsync(Arg.Any<UsersByStatusSpec>(), Arg.Any<CancellationToken>()).Returns(0);
        SetupMapper();

        var result = await _handler.Handle(new AdminListUsersQuery(1, 1, 20, "ACTIVE"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Value.Total.ShouldBe(0);
        result.Value.Pages.ShouldBe(0);
    }
}

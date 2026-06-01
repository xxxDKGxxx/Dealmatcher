namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Bans.Create;

public class CreateBanCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly CreateBanCommandHandler _handler;

    public CreateBanCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateBanCommandHandler(_userRepository, _mapper);
    }

    private static User CreateAdmin(int id = 1)
    {
        var admin = new User("admin@example.com", "hash", "Admin", "User") { Id = id };
        admin.GrantAdminPrivileges();
        return admin;
    }

    private static User CreateRegularUser(int id = 2, string email = "user@example.com")
    {
        return new User(email, "hash", "Regular", "User") { Id = id };
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsSuccessAndSavesToDatabase()
    {
        // Arrange
        var admin = CreateAdmin(1);
        var targetUser = CreateRegularUser(2);
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var command = new CreateBanCommand(1, 2, "Złamanie regulaminu", expiresAt);

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(targetUser);

        _mapper.Map<BanDto>(Arg.Any<Ban>()).Returns(callInfo =>
        {
            var ban = callInfo.Arg<Ban>();
            return new BanDto(1, ban.User.Id, ban.Reason, ban.IssuedBy.Id, ban.IssuedAt, ban.ExpiresAt, ban.IsActive);
        });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Reason.ShouldBe("Złamanie regulaminu");
        targetUser.Status.ShouldBe(UserStatus.Banned);
        targetUser.Bans.Count.ShouldBe(1);

        await _userRepository.Received(1).UpdateAsync(targetUser, Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var command = new CreateBanCommand(99, 2, "Spam", null);
        _userRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsForbidden()
    {
        // Arrange
        var regularUserTryingToBan = CreateRegularUser(1);
        var command = new CreateBanCommand(1, 2, "Spam", null);

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(regularUserTryingToBan);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TargetUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var admin = CreateAdmin(1);
        var command = new CreateBanCommand(1, 99, "Spam", null);

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _userRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TargetUserAlreadyBanned_ReturnsConflict()
    {
        // Arrange
        var admin = CreateAdmin(1);
        var targetUser = CreateRegularUser(2);
        targetUser.BanUser("Poprzedni ban", admin, null);

        var command = new CreateBanCommand(1, 2, "Kolejny powód", null);

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _userRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(targetUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);
        result.Errors.ShouldContain("User is already banned");
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}

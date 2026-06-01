namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Bans.Delete;

public class DeleteBanCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly DeleteBanCommandHandler _handler;

    public DeleteBanCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _handler = new DeleteBanCommandHandler(_userRepository);
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

    [Fact]
    public async Task Handle_ValidData_DeletesBanAndReturnsSuccess()
    {
        // Arrange
        var admin = CreateAdmin(1);
        var targetUser = CreateRegularUser(2);

        targetUser.BanUser("Złamanie regulaminu", admin, null);
        var banId = targetUser.Bans.First().Id;

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByBanIdSpec>(), Arg.Any<CancellationToken>()).Returns(targetUser);

        var command = new DeleteBanCommand(1, banId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        targetUser.Bans.First().IsActive.ShouldBeFalse();
        targetUser.Status.ShouldBe(UserStatus.Active);

        await _userRepository.Received(1).UpdateAsync(targetUser, Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _userRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);
        var command = new DeleteBanCommand(99, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsForbidden()
    {
        var regularUser = CreateRegularUser(1);
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(regularUser);

        var command = new DeleteBanCommand(1, 1);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }

    [Fact]
    public async Task Handle_BanNotFound_ReturnsNotFound()
    {
        var admin = CreateAdmin(1);
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);
        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByBanIdSpec>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var command = new DeleteBanCommand(1, 99);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}

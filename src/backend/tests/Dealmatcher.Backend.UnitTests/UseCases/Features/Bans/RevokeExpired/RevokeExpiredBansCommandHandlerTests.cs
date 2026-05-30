namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Bans.RevokeExpired;

public class RevokeExpiredBansCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly RevokeExpiredBansCommandHandler _handler;

    public RevokeExpiredBansCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _handler = new RevokeExpiredBansCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_UsersWithExpiredBans_RevokesBansAndUpdatesRepository()
    {
        var user = new User("test@example.com", "hash", "Jan", "Kowalski");
        user.BanUser("Wygasły ban", 1, DateTime.UtcNow.AddDays(-1));
        var usersList = new List<User> { user };

        _userRepository.ListAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
            .Returns(usersList);

        var command = new RevokeExpiredBansCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        user.Status.ShouldBe(UserStatus.Active);
        user.Bans.First().IsActive.ShouldBeFalse();

        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoUsersWithExpiredBans_ReturnsSuccessWithoutUpdates()
    {
        _userRepository.ListAsync(Arg.Any<ISpecification<User>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var command = new RevokeExpiredBansCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

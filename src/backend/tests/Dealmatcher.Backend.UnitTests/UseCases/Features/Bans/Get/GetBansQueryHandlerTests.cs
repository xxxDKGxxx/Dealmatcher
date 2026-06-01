namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Bans.Get;

public class GetBansQueryHandlerTests
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly GetBansQueryHandler _handler;

    public GetBansQueryHandlerTests()
    {
        _userRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetBansQueryHandler(_userRepository, _mapper);
    }

    private static User CreateAdmin(int id = 1)
    {
        var admin = new User("admin@example.com", "hash", "Admin", "User") { Id = id };
        admin.GrantAdminPrivileges();
        return admin;
    }

    private static User CreateRegularUser(int id = 2)
    {
        return new User($"user{id}@example.com", "hash", "Regular", "User") { Id = id };
    }

    [Fact]
    public async Task Handle_ValidAdmin_ReturnsMappedBans()
    {
        // Arrange
        var admin = CreateAdmin(1);
        var bannedUser = CreateRegularUser(2);
        bannedUser.BanUser("Spam", admin, null);

        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(admin);

        var usersList = new List<User> { bannedUser };
        _userRepository.ListAsync(Arg.Any<UsersWithFilteredBansSpec>(), Arg.Any<CancellationToken>())
            .Returns(usersList);

        _mapper.Map<BanDto>(Arg.Any<Ban>()).Returns(callInfo =>
        {
            var ban = callInfo.Arg<Ban>();
            return new BanDto(1, ban.User.Id, ban.Reason, ban.IssuedBy.Id, ban.IssuedAt, ban.ExpiresAt, ban.IsActive);
        });

        var query = new GetBansQuery(1, null, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeEmpty();
        result.Value.First().Reason.ShouldBe("Spam");
    }

    [Fact]
    public async Task Handle_AdminNotFound_ReturnsUnauthorized()
    {
        _userRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((User?)null);
        var query = new GetBansQuery(99, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsForbidden()
    {
        var regularUser = CreateRegularUser(1);
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(regularUser);

        var query = new GetBansQuery(1, null, null);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
    }
}

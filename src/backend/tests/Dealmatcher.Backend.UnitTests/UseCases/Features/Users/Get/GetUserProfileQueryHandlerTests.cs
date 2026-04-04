namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Users.Get;

public class GetUserProfileQueryHandlerTests
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUserProfileQueryHandler _handler;

    private const int ValidId = 1;
    private const string ValidEmail = "test@example.com";

    public GetUserProfileQueryHandlerTests()
    {
        _userRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUserProfileQueryHandler(_userRepository, _mapper);
    }

    private static User CreateUser()
    {
        return new User(ValidEmail, "hashed_password", "Jan", "Kowalski");
    }

    private static UserDto CreateUserDto()
    {
        return new UserDto(ValidId, ValidEmail, "Jan", "Kowalski", "ACTIVE", DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_UserExists_ReturnsSuccess()
    {
        var query = new GetUserProfileQuery(ValidId);
        var user = CreateUser();
        var expectedDto = CreateUserDto();

        _userRepository.GetByIdAsync(query.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _mapper.Map<UserDto>(user)
            .Returns(expectedDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);
        result.Value.Id.ShouldBe(ValidId);
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFound()
    {
        var query = new GetUserProfileQuery(ValidId);

        _userRepository.GetByIdAsync(query.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _mapper.DidNotReceive().Map<UserDto>(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_ValidQuery_CallsRepositoryWithCorrectId()
    {
        var query = new GetUserProfileQuery(ValidId);

        _userRepository.GetByIdAsync(ValidId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        await _handler.Handle(query, CancellationToken.None);

        await _userRepository.Received(1).GetByIdAsync(
            ValidId,
            Arg.Any<CancellationToken>());
    }
}

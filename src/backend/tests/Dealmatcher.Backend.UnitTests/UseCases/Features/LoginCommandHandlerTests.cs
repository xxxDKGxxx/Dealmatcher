namespace Dealmatcher.Backend.UnitTests.UseCases.Features;
public class LoginCommandHandlerTests
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly LoginCommandHandler _handler;

    private const string ValidEmail = "test@example.com";
    private const string ValidPassword = "correctpassword";
    private const string ValidPasswordHash = "hashed_password";
    private const string ValidToken = "generated.jwt.token";

    public LoginCommandHandlerTests()
    {
        _userRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _tokenService = Substitute.For<ITokenService>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new LoginCommandHandler(_userRepository, _mapper, _tokenService, _passwordHasher);
    }

    private static User CreateUser(string email = ValidEmail, string passwordHash = ValidPasswordHash)
    {
        return new User(email, passwordHash, "Jan", "Kowalski");
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessWithLoginDto()
    {
        var user = CreateUser();
        var command = new LoginCommand(ValidEmail, ValidPassword);
        var expectedDto = new LoginDto(ValidToken, new UserDto(0, ValidEmail, "Jan", "Kowalski", null, null, null, UserStatus.Active, default));

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(ValidPassword, ValidPasswordHash)
            .Returns(true);
        _tokenService.GenerateToken(user)
            .Returns(ValidToken);
        _mapper.Map<LoginDto>((ValidToken, user))
            .Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);
        result.Value.AccessToken.ShouldBe(ValidToken);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUnauthorized()
    {
        var command = new LoginCommand("nonexistent@example.com", ValidPassword);

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _passwordHasher.DidNotReceive().VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsUnauthorized()
    {
        var user = CreateUser();
        var command = new LoginCommand(ValidEmail, "wrongpassword");

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword("wrongpassword", ValidPasswordHash)
            .Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_BannedUser_ReturnsForbidden()
    {
        var user = CreateUser();
        user.BanUser();
        var command = new LoginCommand(ValidEmail, ValidPassword);

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(ValidPassword, ValidPasswordHash)
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_InactiveUser_ReturnsForbidden()
    {
        var user = CreateUser();
        user.DeactivateUserAccount();
        var command = new LoginCommand(ValidEmail, ValidPassword);

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(ValidPassword, ValidPasswordHash)
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        _tokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_ValidCredentials_CallsServicesInCorrectOrder()
    {
        var user = CreateUser();
        var command = new LoginCommand(ValidEmail, ValidPassword);

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.VerifyPassword(ValidPassword, ValidPasswordHash)
            .Returns(true);
        _tokenService.GenerateToken(user)
            .Returns(ValidToken);
        _mapper.Map<LoginDto>((ValidToken, user))
            .Returns(new LoginDto(ValidToken, new UserDto(0, ValidEmail, "Jan", "Kowalski", null, null, null, UserStatus.Active, default)));

        await _handler.Handle(command, CancellationToken.None);

        Received.InOrder(() =>
        {
            _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>());
            _passwordHasher.VerifyPassword(ValidPassword, ValidPasswordHash);
            _tokenService.GenerateToken(user);
            _mapper.Map<LoginDto>((ValidToken, user));
        });
    }
}

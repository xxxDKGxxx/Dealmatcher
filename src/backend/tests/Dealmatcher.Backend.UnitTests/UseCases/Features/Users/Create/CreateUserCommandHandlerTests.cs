namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Users.Create;

public class CreateUserCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly CreateUserCommandHandler _handler;

    private const string ValidEmail = "test@example.com";
    private const string ValidPassword = "correctpassword!";
    private const string ValidPasswordHash = "hashed_password";
    private const int ValidId = 1;

    public CreateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _handler = new CreateUserCommandHandler(_userRepository, _mapper, _passwordHasher);
    }

    private static User CreateUser(string email = ValidEmail)
    {
        return new User(email, ValidPasswordHash, "Jan", "Kowalski");
    }

    private static UserDto CreateUserDto(string email = ValidEmail)
    {
        return new UserDto(ValidId, email, "Jan", "Kowalski", null, null, null, UserStatus.Active, default);
    }

    [Fact]
    public async Task Handle_ValidData_CreatesUserAndReturnsSuccess()
    {
        var command = new CreateUserCommand(ValidEmail, ValidPassword, "Jan", "Kowalski");
        var expectedDto = CreateUserDto();

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.HashPassword(ValidPassword)
            .Returns(ValidPasswordHash);
        _mapper.Map<UserDto>(Arg.Any<BasicUser>())
            .Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);
        result.Value.Email.ShouldBe(ValidEmail);
    }

    [Fact]
    public async Task Handle_EmailAlreadyTaken_ReturnsConflict()
    {
        var command = new CreateUserCommand(ValidEmail, ValidPassword, "Jan", "Kowalski");
        var existingUser = CreateUser();

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Conflict);
        result.Errors.ShouldContain("Email is already taken");

        _passwordHasher.DidNotReceive().HashPassword(Arg.Any<string>());
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<BasicUser>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithUnnormalizedEmail_NormalizesEmailBeforeSaving()
    {
        var command = new CreateUserCommand("Jan.Kowalski@EMAIL.com", ValidPassword, "Jan", "Kowalski");
        const string NormalizedEmail = "jan.kowalski@email.com";

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.HashPassword(ValidPassword)
            .Returns(ValidPasswordHash);
        _mapper.Map<UserDto>(Arg.Any<BasicUser>())
            .Returns(CreateUserDto(NormalizedEmail));

        await _handler.Handle(command, CancellationToken.None);

        await _userRepository.Received(1).AddAsync(
            Arg.Is<BasicUser>(u =>
                u.Email == NormalizedEmail &&
                u.Name == "Jan" &&
                u.Surname == "Kowalski"),
            Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidData_CallsServicesInCorrectOrder()
    {
        var command = new CreateUserCommand(ValidEmail, ValidPassword, "Jan", "Kowalski");

        _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.HashPassword(ValidPassword)
            .Returns(ValidPasswordHash);
        _mapper.Map<UserDto>(Arg.Any<BasicUser>())
            .Returns(CreateUserDto());

        await _handler.Handle(command, CancellationToken.None);

        Received.InOrder(async () =>
        {
            await _userRepository.SingleOrDefaultAsync(Arg.Any<UserByEmailSpec>(), Arg.Any<CancellationToken>());
            _passwordHasher.HashPassword(ValidPassword);
            await _userRepository.AddAsync(Arg.Any<BasicUser>(), Arg.Any<CancellationToken>());
            await _userRepository.SaveChangesAsync(Arg.Any<CancellationToken>());
            _mapper.Map<UserDto>(Arg.Any<BasicUser>());
        });
    }
}

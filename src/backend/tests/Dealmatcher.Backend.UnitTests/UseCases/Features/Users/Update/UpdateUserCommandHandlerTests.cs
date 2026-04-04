namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Users.Update;

public class UpdateUserCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly UpdateUserCommandHandler _handler;

    private const int ValidId = 1;
    private const string ValidEmail = "test@example.com";
    private const string OldName = "Jan";
    private const string OldSurname = "Kowalski";
    private const string NewName = "Adam";
    private const string NewSurname = "Nowak";

    public UpdateUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateUserCommandHandler(_userRepository, _mapper);
    }

    private static User CreateUser()
    {
        return new User(ValidEmail, "hashed_password", OldName, OldSurname);
    }

    private static UserDto CreateUserDto()
    {
        return new UserDto(ValidId, ValidEmail, NewName, NewSurname, UserStatus.Active.Name, DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_UserExists_UpdatesDataAndReturnsSuccessWithDto()
    {
        var command = new UpdateUserCommand(ValidId, NewName, NewSurname);
        var user = CreateUser();
        var expectedDto = CreateUserDto();

        _userRepository.GetByIdAsync(ValidId, Arg.Any<CancellationToken>())
            .Returns(user);

        _mapper.Map<UserDto>(user).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        result.Value.ShouldBe(expectedDto);

        user.Name.ShouldBe(NewName);
        user.Surname.ShouldBe(NewSurname);

        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundAndDoesNotSave()
    {
        var command = new UpdateUserCommand(ValidId, NewName, NewSurname);

        _userRepository.GetByIdAsync(ValidId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);

        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        _mapper.DidNotReceive().Map<UserDto>(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsServicesInCorrectOrder()
    {
        var command = new UpdateUserCommand(ValidId, NewName, NewSurname);
        var user = CreateUser();
        var expectedDto = CreateUserDto();

        _userRepository.GetByIdAsync(ValidId, Arg.Any<CancellationToken>())
            .Returns(user);

        _mapper.Map<UserDto>(user).Returns(expectedDto);

        await _handler.Handle(command, CancellationToken.None);

        Received.InOrder(async () =>
        {
            await _userRepository.GetByIdAsync(ValidId, Arg.Any<CancellationToken>());
            await _userRepository.UpdateAsync(user, Arg.Any<CancellationToken>());
            await _userRepository.SaveChangesAsync(Arg.Any<CancellationToken>());
            _mapper.Map<UserDto>(user);
        });
    }
}

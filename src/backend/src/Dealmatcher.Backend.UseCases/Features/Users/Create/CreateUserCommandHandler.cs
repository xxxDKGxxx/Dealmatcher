namespace Dealmatcher.Backend.UseCases.Features.Users.Create;

public sealed class CreateUserCommandHandler(
    IRepository<User> userRepository,
    IMapper mapper,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var spec = new UserByEmailSpec(normalizedEmail);
        var existingUsers = await userRepository.ListAsync(spec, cancellationToken);

        bool isEmailTakenByActiveUser = existingUsers.Any(u => u.Status == UserStatus.Active);

        if (isEmailTakenByActiveUser)
        {
            return Result.Conflict("Email is already taken by an active account");
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);

        var newUser = new BasicUser(normalizedEmail, passwordHash, request.Name, request.Surname);

        await userRepository.AddAsync(newUser, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var userDto = mapper.Map<UserDto>(newUser);
        return Result.Success(userDto);
    }
}

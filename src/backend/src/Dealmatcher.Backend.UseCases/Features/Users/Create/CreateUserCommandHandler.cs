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
        var existingUser = await userRepository.SingleOrDefaultAsync(spec, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Conflict("Email is already taken");
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);

        var newUser = new BasicUser(normalizedEmail, passwordHash, request.Name, request.Surname);

        await userRepository.AddAsync(newUser, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var userDto = mapper.Map<UserDto>(newUser);
        return Result.Success(userDto);
    }
}

namespace Dealmatcher.Backend.UseCases.Features.Users.Create;

public sealed class CreateUserCommandHandler(
    IRepository<User> userRepository,
    IMapper mapper,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim()
            .ToLowerInvariant();

        var spec = new ActiveOrBannedUserByEmailSpec(normalizedEmail);
        var conflictingUser = await userRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (conflictingUser is not null)
        {
            return Result.Conflict("Email is already taken by an active or banned account");
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);
        var newUser = new BasicUser(normalizedEmail, passwordHash, request.Name, request.Surname);

        await userRepository.AddAsync(newUser, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var userDto = mapper.Map<UserDto>(newUser);
        return Result.Success(userDto);
    }
}

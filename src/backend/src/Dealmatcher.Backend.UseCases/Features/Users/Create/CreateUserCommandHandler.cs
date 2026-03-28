namespace Dealmatcher.Backend.UseCases.Features.Users.Create;

public sealed class CreateUserCommandHandler(
    IRepository<User> userRepository,
    IMapper mapper,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserByEmailSpec(request.Email);
        var existingUser = await userRepository.SingleOrDefaultAsync(spec, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Conflict("Email is already taken");
        }

        var passwordHash = passwordHasher.HashPassword(request.Password);

        var newUser = new BasicUser(request.Email, passwordHash, request.Name, request.Surname);

        await userRepository.AddAsync(newUser, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var userDto = mapper.Map<UserDto>(newUser);
        return Result.Success(userDto);
    }
}

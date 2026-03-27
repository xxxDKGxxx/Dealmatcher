namespace Dealmatcher.Backend.UseCases.Features.Authentication.Login;

public sealed class LoginCommandHandler(
    IReadRepository<UserEntity> userRepository,
    IMapper mapper,
    ITokenService tokenService,
    IPasswordHasher passwordHasher) : ICommandHandler<LoginCommand, Result<LoginDto>>
{
    public async Task<Result<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserByEmailSpec(request.Email);
        var user = await userRepository.SingleOrDefaultAsync(spec, cancellationToken);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Unauthorized();
        }

        if (!user.Status.CanLogin)
        {
            return Result.Forbidden();
        }

        var token = tokenService.GenerateToken(user);

        return mapper.Map<LoginDto>((token, user));
    }
}

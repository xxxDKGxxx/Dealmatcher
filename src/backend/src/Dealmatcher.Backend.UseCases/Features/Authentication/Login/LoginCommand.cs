namespace Dealmatcher.Backend.UseCases.Features.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<Result<LoginDto>>;

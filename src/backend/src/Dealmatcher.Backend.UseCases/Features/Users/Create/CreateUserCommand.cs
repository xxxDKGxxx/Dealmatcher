namespace Dealmatcher.Backend.UseCases.Features.Users.Create;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string Name,
    string Surname
) : ICommand<Result<UserDto>>;

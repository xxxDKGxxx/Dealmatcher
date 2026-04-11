namespace Dealmatcher.Backend.UseCases.Features.Users.Update;

public record UpdateUserCommand(
    int UserId,
    string Name,
    string Surname) : ICommand<Result<UserDto>>;

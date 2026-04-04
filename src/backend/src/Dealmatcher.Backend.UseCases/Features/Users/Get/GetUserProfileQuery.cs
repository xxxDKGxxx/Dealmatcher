namespace Dealmatcher.Backend.UseCases.Features.Users.Get;
public record GetUserProfileQuery(int UserId) : IQuery<Result<UserDto>>;

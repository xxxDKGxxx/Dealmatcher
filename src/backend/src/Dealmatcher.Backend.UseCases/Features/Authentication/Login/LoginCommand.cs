namespace Dealmatcher.Backend.UseCases.Features.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<Result<LoginDto>>, ILoggableActivity<Result<LoginDto>>
{
    public ActivityAction Action => ActivityAction.Login;
    public Dictionary<string, string> GetDetails(Result<LoginDto> result) => [];
    public int? GetOfferId(Result<LoginDto> result) => null;
    public int? GetUserId(Result<LoginDto> result) => result.Value.User.Id;
}

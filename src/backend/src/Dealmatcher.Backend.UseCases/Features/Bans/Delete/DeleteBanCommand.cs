namespace Dealmatcher.Backend.UseCases.Features.Bans.Delete;

public sealed record DeleteBanCommand(
    int AdminId,
    int BanId
) : ICommand<Result>, ILoggableActivity<Result>
{
    public ActivityAction Action => ActivityAction.StatusChange;
    public Dictionary<string, string> GetDetails(Result result) => new()
    {
        ["banId"] = BanId.ToString(),
        ["action"] = "Revoked"
    };
    public int? GetOfferId(Result result) => null;
    public int? GetUserId(Result result) => AdminId;
}

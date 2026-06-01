namespace Dealmatcher.Backend.UseCases.Features.Bans.Create;

public sealed record CreateBanCommand(
    int AdminId,
    int UserId,
    string Reason,
    DateTime? ExpiresAt
) : ICommand<Result<BanDto>>, ILoggableActivity<Result<BanDto>>
{
    public ActivityAction Action => ActivityAction.StatusChange;
    public Dictionary<string, string> GetDetails(Result<BanDto> result) => new()
    {
        ["reason"] = Reason
    };
    public int? GetOfferId(Result<BanDto> result) => null;
    public int? GetUserId(Result<BanDto> result) => UserId;
}

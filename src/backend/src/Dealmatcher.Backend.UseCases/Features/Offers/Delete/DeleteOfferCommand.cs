namespace Dealmatcher.Backend.UseCases.Features.Offers.Delete;

public sealed record DeleteOfferCommand(int OfferId, int UserId) : ICommand<Result>, ILoggableActivity<Result>
{
    public ActivityAction Action => ActivityAction.Delete;
    public Dictionary<string, string> GetDetails(Result result) => [];
    public int? GetOfferId(Result result) => OfferId;
    public int? GetUserId(Result result) => UserId;
}

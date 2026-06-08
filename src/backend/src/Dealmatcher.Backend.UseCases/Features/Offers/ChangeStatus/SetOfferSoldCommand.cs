namespace Dealmatcher.Backend.UseCases.Features.Offers.ChangeStatus;

public sealed record SetOfferSoldCommand(int userId, int offerId) : ICommand<Result<OfferDto>>, ILoggableActivity<Result<OfferDto>>
{
    public ActivityAction Action => ActivityAction.StatusChange;
    public Dictionary<string, string> GetDetails(Result<OfferDto> result) => [];
    public int? GetOfferId(Result<OfferDto> result) => offerId;
    public int? GetUserId(Result<OfferDto> result) => userId;
}

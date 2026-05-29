namespace Dealmatcher.Backend.UseCases.Features.Offers.Get;

public record GetOfferQuery(int OfferId, int? userId = null) : IQuery<Result<OfferDto>>, ILoggableActivity<Result<OfferDto>>
{
    public ActivityAction Action => ActivityAction.View;
    public Dictionary<string, string> GetDetails(Result<OfferDto> result) => [];
    public int? GetOfferId(Result<OfferDto> result) => OfferId;
    public int? GetUserId(Result<OfferDto> result) => userId;
}

namespace Dealmatcher.Backend.UseCases.Features.Offers.Update;

public sealed record UpdateOfferCommand(
    int OfferId,
    int UserId,
    string? Title,
    string? Description,
    decimal? Price,
    List<string>? Images,
    List<string>? Tags,
    Dictionary<string, string>? Properties,
    int? Availability) : ICommand<Result<OfferDto>>, ILoggableActivity<Result<OfferDto>>
{
    public ActivityAction Action => ActivityAction.Update;
    public Dictionary<string, string> GetDetails(Result<OfferDto> result) => [];
    public int? GetOfferId(Result<OfferDto> result) => OfferId;
    public int? GetUserId(Result<OfferDto> result) => UserId;
}

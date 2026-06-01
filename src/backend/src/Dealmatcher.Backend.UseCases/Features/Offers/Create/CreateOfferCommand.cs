namespace Dealmatcher.Backend.UseCases.Features.Offers.Create;

public sealed record CreateOfferCommand(
    string Title,
    string Description,
    decimal Price,
    List<FileDto> Images,
    int SellerId,
    int CategoryId,
    List<string> Tags,
    Dictionary<string, string> Properties,
    int Availability) : ICommand<Result<OfferDto>>, ILoggableActivity<Result<OfferDto>>
{
    public ActivityAction Action => ActivityAction.Create;
    public Dictionary<string, string> GetDetails(Result<OfferDto> result) => [];
    public int? GetOfferId(Result<OfferDto> result) => result.Value.Id;
    public int? GetUserId(Result<OfferDto> result) => SellerId;
}

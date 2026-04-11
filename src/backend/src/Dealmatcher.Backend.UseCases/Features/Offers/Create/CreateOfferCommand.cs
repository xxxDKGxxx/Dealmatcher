namespace Dealmatcher.Backend.UseCases.Features.Offers.Create;

public sealed record CreateOfferCommand(
    string Title,
    string Description,
    decimal Price,
    //TODO: List<Image> images,
    int SellerId,
    int CategoryId,
    List<string> Tags,
    Dictionary<string, string> Properties,
    int Availability) : ICommand<Result<OfferDto>>;

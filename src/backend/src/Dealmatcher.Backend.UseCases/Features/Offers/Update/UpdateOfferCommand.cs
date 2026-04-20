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
    int? Availability) : ICommand<Result<OfferDto>>;

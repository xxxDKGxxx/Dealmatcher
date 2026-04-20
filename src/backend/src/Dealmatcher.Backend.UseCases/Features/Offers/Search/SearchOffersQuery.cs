namespace Dealmatcher.Backend.UseCases.Features.Offers.Search;

public sealed record SearchOffersQuery(
    int CategoryId,
    decimal MinPrice,
    decimal MaxPrice,
    List<string> Tags,
    string SearchPhrase,
    int limit): IQuery<Result<List<OfferDto>>>;

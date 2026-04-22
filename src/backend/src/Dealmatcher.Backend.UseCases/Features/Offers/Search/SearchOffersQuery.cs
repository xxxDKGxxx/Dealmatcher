namespace Dealmatcher.Backend.UseCases.Features.Offers.Search;

public sealed record SearchOffersQuery(
    int? CategoryId,
    decimal MinPrice,
    decimal MaxPrice,
    List<string> Tags,
    Dictionary<string, List<string>> PropertyFilters,
    string SearchPhrase,
    int Limit) : IQuery<Result<List<OfferDto>>>;

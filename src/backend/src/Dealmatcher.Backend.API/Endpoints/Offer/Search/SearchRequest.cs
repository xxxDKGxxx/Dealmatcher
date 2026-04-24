namespace Dealmatcher.Backend.API.Endpoints.Offer.Search;

public sealed record SearchRequest(
    int? CategoryId,
    decimal MinPrice,
    decimal MaxPrice,
    List<string> Tags,
    Dictionary<string, List<string>> Properties,
    string SearchPhrase,
    int Limit);

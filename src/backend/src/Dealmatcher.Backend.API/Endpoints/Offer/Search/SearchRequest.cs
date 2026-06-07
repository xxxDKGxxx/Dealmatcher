namespace Dealmatcher.Backend.API.Endpoints.Offer.Search;

public sealed record SearchRequest
{
    public int? CategoryId { get; init; } = null;
    public decimal MinPrice { get; init; } = 0;
    public decimal MaxPrice { get; init; } = decimal.MaxValue;
    public List<string> Tags { get; init; } = [];
    public Dictionary<string, List<string>> Properties { get; init; } = [];
    public string SearchPhrase { get; init; } = "";
    public int Limit { get; init; } = 20;
}

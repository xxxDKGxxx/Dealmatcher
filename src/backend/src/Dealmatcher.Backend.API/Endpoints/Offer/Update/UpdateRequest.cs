namespace Dealmatcher.Backend.API.Endpoints.Offer.Update;

public sealed record UpdateRequest
{
    public int OfferId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public List<string>? Images { get; init; }
    public List<string>? Tags { get; init; }
    public Dictionary<string, string>? Properties { get; init; }
    public int? Availability { get; init; }
}

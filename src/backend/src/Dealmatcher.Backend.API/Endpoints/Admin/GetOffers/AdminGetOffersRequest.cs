namespace Dealmatcher.Backend.API.Endpoints.Admin.GetOffers;

public sealed record AdminGetOffersRequest
{
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = 20;
    public string Status { get; init; } = "DRAFT";
}

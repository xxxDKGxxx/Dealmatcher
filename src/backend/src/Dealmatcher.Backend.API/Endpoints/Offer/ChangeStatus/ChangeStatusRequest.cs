namespace Dealmatcher.Backend.API.Endpoints.Offer.ChangeStatus;

public sealed class ChangeStatusRequest
{
    public int OfferId { get; init; }

    public string Status { get; init; } = string.Empty;
}

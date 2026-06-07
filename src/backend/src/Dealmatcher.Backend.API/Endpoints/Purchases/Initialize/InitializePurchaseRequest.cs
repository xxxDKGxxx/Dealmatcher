namespace Dealmatcher.Backend.API.Endpoints.Purchases.Initialize;

public sealed record InitializePurchaseRequest
{
    public int OfferId { get; init; }
    public string DeliveryMethodId { get; init; } = null!;
    public string PaymentMethodId { get; init; } = null!;
    public int Quantity { get; init; } = 1;
}

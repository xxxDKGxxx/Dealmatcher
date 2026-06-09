namespace Dealmatcher.Backend.API.Endpoints.Purchases.ProcessPayment;

public sealed record ProcessPaymentRequest
{
    public string SessionId { get; init; } = null!;
}

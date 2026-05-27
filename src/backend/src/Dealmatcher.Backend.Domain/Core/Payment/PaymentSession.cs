namespace Dealmatcher.Backend.Domain.Core.Payment;

public sealed record PaymentSession(
    string ProviderName,
    string SessionId,
    string RedirectUrl,
    decimal Amount,
    string Currency);

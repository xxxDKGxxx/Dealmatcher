namespace Dealmatcher.Backend.Domain.Core.Payment;

public sealed record PaymentMethodDto(
    string Id,
    string Name,
    string Provider,
    string Icon);

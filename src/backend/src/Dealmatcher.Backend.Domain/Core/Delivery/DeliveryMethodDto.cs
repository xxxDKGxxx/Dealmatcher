namespace Dealmatcher.Backend.Domain.Core.Delivery;

public sealed record DeliveryMethodDto(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int EstimatedDays
);

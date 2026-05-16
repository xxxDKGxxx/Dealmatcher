namespace Dealmatcher.Backend.Domain.EntityAggregates.DeliveryMethodAggregate.Dto;

public sealed record DeliveryMethodDto(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int EstimatedDays
);

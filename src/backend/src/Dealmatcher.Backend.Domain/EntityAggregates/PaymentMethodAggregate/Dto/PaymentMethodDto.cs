namespace Dealmatcher.Backend.Domain.EntityAggregates.PaymentMethodAggregate.Dto;

public sealed record PaymentMethodDto(
    string Id,
    string Name,
    string Provider,
    string Icon);

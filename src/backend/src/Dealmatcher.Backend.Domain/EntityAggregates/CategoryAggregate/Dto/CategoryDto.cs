namespace Dealmatcher.Backend.Domain.EntityAggregates.CategoryAggregate.Dto;

public sealed record CategoryDto(
    int Id,
    string Name,
    string Description);

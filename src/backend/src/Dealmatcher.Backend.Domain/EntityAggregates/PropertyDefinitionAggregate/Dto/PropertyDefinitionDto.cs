namespace Dealmatcher.Backend.Domain.EntityAggregates.PropertyDefinitionAggregate.Dto;

public sealed record PropertyDefinitionDto(
    int Id,
    string Name,
    string Type,
    CategoryRelatedEnumDto? CategoryRelatedEnum);

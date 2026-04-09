namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions.Dto;

public sealed record PropertyDefinitionDto(
    int Id,
    string Name,
    PropertyType Type,
    PropertyRelatedEnumDto? PropertyRelatedEnum,
    List<string>? options);

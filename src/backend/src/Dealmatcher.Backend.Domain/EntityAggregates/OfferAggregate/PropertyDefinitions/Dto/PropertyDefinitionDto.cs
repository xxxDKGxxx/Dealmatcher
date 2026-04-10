namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyDefinitions.Dto;

public sealed record PropertyDefinitionDto(
    int Id,
    string Name,
    string Type,
    List<string>? options);

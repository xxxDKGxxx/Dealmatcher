namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.PropertyRelatedEnums.Dto;

public sealed record PropertyRelatedEnumDto(
    int Id,
    string Name,
    List<string> Options);

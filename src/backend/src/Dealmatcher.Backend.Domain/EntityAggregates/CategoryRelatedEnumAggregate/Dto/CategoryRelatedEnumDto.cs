namespace Dealmatcher.Backend.Domain.EntityAggregates.CategoryRelatedEnumAggregate.Dto;

public sealed record CategoryRelatedEnumDto(
    int Id,
    string Name,
    List<string> Options);

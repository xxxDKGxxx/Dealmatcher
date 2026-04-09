namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Categories.Dto;

public sealed record CategoryDto(
    int Id,
    string Name,
    string Description);

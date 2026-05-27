namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Dto;

public sealed record AdminOffersPageDto(
    List<OfferDto> Items,
    int Total,
    int Page,
    int Pages);

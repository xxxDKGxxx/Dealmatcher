namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Dto;

public sealed record OfferDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    List<string> Images,
    SellerDto Seller,
    CategoryDto Category,
    List<string> Tags,
    List<PropertyDto> Properties,
    int Availability,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

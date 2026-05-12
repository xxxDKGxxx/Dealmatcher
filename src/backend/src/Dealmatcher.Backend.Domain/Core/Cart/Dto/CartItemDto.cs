namespace Dealmatcher.Backend.Domain.Core.Cart.Dto;

public sealed record CartItemDto(
    int Id,
    OfferDto Offer,
    int Quantity,
    DateTime AddedAt
);

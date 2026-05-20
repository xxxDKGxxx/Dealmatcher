namespace Dealmatcher.Backend.Domain.Interfaces.Delivery;

public sealed record DeliveryContext(
    User Buyer,
    User Seller,
    int OfferId,
    DateTime RequestTime
);

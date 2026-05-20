namespace Dealmatcher.Backend.Domain.Core.Delivery;

public sealed record DeliveryContext(
    User Buyer,
    User Seller,
    int OfferId,
    DateTime RequestTime
);

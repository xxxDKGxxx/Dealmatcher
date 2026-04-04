namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Dto;

public sealed record UserDetailDto(
    int Id,
    string Email,
    string Name,
    string Surname,
    UserStatus Status,
    DateTime CreatedAt,
    int TotalOffers,
    int TotalSales,
    int TotalPurchases,
    DateTime? LastActive
);

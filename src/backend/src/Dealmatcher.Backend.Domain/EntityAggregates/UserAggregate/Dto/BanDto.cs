namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Dto;

public sealed record BanDto(
    int Id,
    int UserId,
    string Reason,
    int IssuedBy,
    DateTime IssuedAt,
    DateTime? ExpiresAt,
    bool IsActive
);

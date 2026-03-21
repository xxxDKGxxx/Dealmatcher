namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Dto;

public sealed record UserDto(
    int Id,
    string Email,
    string Name,
    string Surname,
    string? Phone,
    string? Address,
    float? Rating,
    UserStatus Status,
    DateTime CreatedAt
);

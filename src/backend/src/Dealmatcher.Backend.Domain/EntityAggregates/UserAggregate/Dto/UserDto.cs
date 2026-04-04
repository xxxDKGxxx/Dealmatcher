namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Dto;

public sealed record UserDto(
    int Id,
    string Email,
    string Name,
    string Surname,
    UserStatus Status,
    DateTime CreatedAt
);

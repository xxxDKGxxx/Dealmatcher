namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate.Dto;

public sealed record AdminUsersPageDto(
    List<UserDto> Items,
    int Total,
    int Page,
    int Pages);

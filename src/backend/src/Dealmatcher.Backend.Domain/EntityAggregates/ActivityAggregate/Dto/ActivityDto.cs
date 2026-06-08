namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate.Dto;

public sealed record ActivityDto(
    int Id,
    int UserId,
    int? OfferId,
    string Action,
    Dictionary<string, string> Details,
    string IPAddress,
    DateTime CreatedAt);

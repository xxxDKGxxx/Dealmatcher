namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages.Dto;

public sealed record MessageDto(
  int Id,
  int SenderId,
  string Content,
  string Status,
  DateTime CreatedAt
);

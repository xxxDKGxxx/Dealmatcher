namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Dto;

public sealed record ConversationDto(
  int Id,
  OfferDto Offer,
  ConversationParticipantDto Buyer,
  ConversationParticipantDto Seller,
  string LastMessage,
  DateTime LastMessageAt,
  int UnreadCount,
  string Status,
  DateTime CreatedAt
);

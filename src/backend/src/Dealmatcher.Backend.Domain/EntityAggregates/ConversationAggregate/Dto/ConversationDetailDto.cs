using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages.Dto;

namespace Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Dto;

public sealed record ConversationDetailDto(
  OfferDto Offer,
  ConversationParticipantDto Buyer,
  ConversationParticipantDto Seller,
  string LastMessage,
  DateTime LastMessageAt,
  int UnreadCount,
  string Status,
  DateTime CreatedAt,
  ICollection<MessageDto> Messages
);

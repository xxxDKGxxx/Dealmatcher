import 'package:frontend/models/message.dart';
import 'package:frontend/models/offer.dart';

class ConversationParticipant {
  const ConversationParticipant({required this.id, required this.name});

  final int id;
  final String name;

  factory ConversationParticipant.fromJson(Map<String, dynamic> json) {
    return ConversationParticipant(
      id: json['id'] as int,
      name: json['name'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'name': name};
  }
}

class ConversationDetail {
  const ConversationDetail({
    required this.id,
    required this.offer,
    required this.buyer,
    required this.seller,
    required this.lastMessage,
    required this.lastMessageAt,
    required this.unreadCount,
    required this.status,
    required this.createdAt,
    required this.messages,
  });

  final int id;
  final Offer offer;
  final ConversationParticipant buyer;
  final ConversationParticipant seller;
  final String lastMessage;
  final DateTime lastMessageAt;
  final int unreadCount;
  final String status;
  final DateTime createdAt;
  final List<Message> messages;

  factory ConversationDetail.fromJson(Map<String, dynamic> json) {
    return ConversationDetail(
      id: json['id'] as int,
      offer: Offer.fromJson(json['offer'] as Map<String, dynamic>),
      buyer: ConversationParticipant.fromJson(
        json['buyer'] as Map<String, dynamic>,
      ),
      seller: ConversationParticipant.fromJson(
        json['seller'] as Map<String, dynamic>,
      ),
      lastMessage: json['lastMessage'] as String,
      lastMessageAt: DateTime.parse(json['lastMessageAt'] as String),
      unreadCount: json['unreadCount'] as int,
      status: json['status'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
      messages: json['messages'] == null
          ? []
          : (json['messages'] as List<dynamic>)
                .map((e) => Message.fromJson(e as Map<String, dynamic>))
                .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'offer': offer.toJson(),
      'buyer': buyer.toJson(),
      'seller': seller.toJson(),
      'lastMessage': lastMessage,
      'lastMessageAt': lastMessageAt.toIso8601String(),
      'unreadCount': unreadCount,
      'status': status,
      'createdAt': createdAt.toIso8601String(),
      'messages': messages.map((e) => e.toJson()).toList(),
    };
  }
}

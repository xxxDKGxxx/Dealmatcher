import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/message.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/category.dart';

class ApiConversations {
  Future<ConversationDetail> getConversation(int id) async {
    // Mocking the response for now
    await Future.delayed(const Duration(milliseconds: 500));

    // Sample data to match ConversationDetailDto
    return ConversationDetail(
      id: id,
      offer: Offer(
        id: 1,
        title: "Przykładowa oferta",
        description: "Opis przykładowej oferty",
        price: 100.0,
        images: [],
        seller: const Seller(id: 2, name: "Sprzedawca"),
        category: Category(id: 1, name: "Elektronika", description: ""),
        tags: ["tag1"],
        properties: {},
        availability: 1,
        status: OfferStatus.active,
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
      ),
      buyer: const ConversationParticipant(id: 1, name: "Kupujący"),
      seller: const ConversationParticipant(id: 2, name: "Sprzedawca"),
      lastMessage: "Cześć, czy oferta aktualna?",
      lastMessageAt: DateTime.now(),
      unreadCount: 0,
      status: "Active",
      createdAt: DateTime.now(),
      messages: [
        Message(
          id: 1,
          senderId: 1,
          content: "Cześć, czy oferta aktualna?",
          status: "Read",
          createdAt: DateTime.now().subtract(const Duration(minutes: 10)),
        ),
        Message(
          id: 2,
          senderId: 2,
          content: "Tak, zapraszam do zakupu.",
          status: "Read",
          createdAt: DateTime.now().subtract(const Duration(minutes: 5)),
        ),
      ],
    );
  }

  Future<void> sendMessage(int conversationId, String content) async {
    // Mocking send message
    await Future.delayed(const Duration(milliseconds: 300));
  }
}

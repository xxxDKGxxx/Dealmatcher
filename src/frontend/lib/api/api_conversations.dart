import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/conversation_create_request.dart';
import 'package:frontend/api/models/conversation_list_response.dart';
import 'package:frontend/api/models/conversation_response.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/message.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/category.dart';

class ApiConversations {
  final _apiCore = ApiCore();
  final _apiCreateConversationUrl = ApiUrls().conversations;
  final _apiGetConversationsUrl = ApiUrls().conversations;

  Future<int> createConversation(int offerId, String initialMessage) async {
    try {
      final request = ConversationCreateRequest(
        offerId: offerId,
        initialMessage: initialMessage,
      );
      final response = await _apiCore.post(_apiCreateConversationUrl, request);

      switch (response.statusCode) {
        case 200:
        case 201:
          {
            final responseModel = ConversationResponse(response: response);
            responseModel.fromJson();
            return responseModel.conversationDetail.id;
          }
        case 400:
          throw Exception('Invalid request');
        case 401:
          throw Exception('Unauthorized');
        case 403:
          throw Exception('Cannot create conversation with yourself');
        case 404:
          throw Exception('Offer not found');
        case 409:
          throw Exception('Conversation already exists');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

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

  Future<ConversationDetail?> getConversationByOfferId(int offerId) async {
    final conversations = await getConversations();
    final offerConversations = conversations.where(
      (c) => c.offer.id == offerId,
    );
    return offerConversations.isEmpty ? null : offerConversations.first;
  }

  Future<List<ConversationDetail>> getConversations() async {
    try {
      final response = await _apiCore.get(_apiGetConversationsUrl);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = ConversationListResponse(response: response);
            responseModel.fromJson();
            return responseModel.conversations;
          }
        case 401:
          throw Exception('Unauthorized');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<void> sendMessage(int conversationId, String content) async {
    // Mocking send message
    await Future.delayed(const Duration(milliseconds: 300));
  }
}

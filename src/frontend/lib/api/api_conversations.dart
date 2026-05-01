import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/conversation_create_request.dart';
import 'package:frontend/api/models/conversation_response.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/message.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/category.dart';

class ApiConversations {
  final _apiCore = ApiCore();
  final _apiCreateConversationUrl = ApiUrls().createConversation;

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

  Future<List<ConversationDetail>> getConversations() async {
    // Mocking the response for now
    await Future.delayed(const Duration(milliseconds: 500));

    // Mock conversation list
    return List.generate(10000, (index) {
      final conversationId = index;
      final offerId = index;

      return ConversationDetail(
        id: conversationId,
        offer: Offer(
          id: offerId,
          title: "iPhone 13 Pro 128GB",
          description: "Stan idealny, bateria 90%.",
          price: 2500.0,
          images: ["https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi.ytimg.com%2Fvi%2Fp3EqJ1qdys4%2Fmaxresdefault.jpg&f=1&nofb=1&ipt=db28530fc7b7411f2fc387c393b3096da6a492db4a5f95a9d191c6f2ddce9b16"],
          seller: const Seller(id: 20, name: "Marek Tech"),
          category: Category(id: 2, name: "Elektronika", description: "Smartfony"),
          tags: ["apple", "iphone"],
          properties: {0: "Graphite"},
          availability: 1,
          status: OfferStatus.active,
          createdAt: DateTime.now().subtract(const Duration(days: 2)),
          updatedAt: DateTime.now().subtract(const Duration(days: 1)),
        ),
        buyer: const ConversationParticipant(id: 1, name: "Jan Kowalski"),
        seller: const ConversationParticipant(id: 20, name: "Marek Tech"),
        // Ostatnia wiadomość zawiera ID konwersacji i oferty
        lastMessage: "Konwersacja: $conversationId, Oferta: $offerId",
        lastMessageAt: DateTime.now().subtract(Duration(minutes: 15 + index)),
        unreadCount: 1,
        status: "Active",
        createdAt: DateTime.now().subtract(const Duration(hours: 5)),
        messages: [
          Message(
            id: 501 + (index * 2),
            senderId: 1,
            content: "Dzień dobry, czy cena jest do negocjacji?",
            status: "Read",
            createdAt: DateTime.now().subtract(const Duration(hours: 1)),
          ),
          Message(
            id: 502 + (index * 2),
            senderId: 1,
            content: "Konwersacja: $conversationId, Oferta: $offerId",
            status: "Sent",
            createdAt: DateTime.now().subtract(Duration(minutes: 15 + index)),
          ),
        ],
      );
    });
  }

  Future<void> sendMessage(int conversationId, String content) async {
    // Mocking send message
    await Future.delayed(const Duration(milliseconds: 300));
  }
}

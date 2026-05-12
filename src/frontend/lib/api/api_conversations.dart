import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/conversation_create_request.dart';
import 'package:frontend/api/models/conversation_list_response.dart';
import 'package:frontend/api/models/conversation_response.dart';
import 'package:frontend/api/models/message_response.dart';
import 'package:frontend/api/models/send_message_request.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/message.dart';

class ApiConversations {
  final _apiCore = ApiCore();
  final _apiCreateConversationUrl = ApiUrls().conversations;
  final _apiGetConversationsUrl = ApiUrls().conversations;

  Future<ConversationDetail> createConversation(
    int offerId,
    String initialMessage,
  ) async {
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
            return responseModel.conversationDetail;
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
    try {
      final response = await _apiCore.get(ApiUrls().conversationById(id));

      switch (response.statusCode) {
        case 200:
        case 201:
          {
            final responseModel = ConversationResponse(response: response);
            responseModel.fromJson();
            return responseModel.conversationDetail;
          }
        case 401:
          throw Exception('Unauthorized');
        case 403:
          throw Exception('Forbidden - not your conversation');
        case 404:
          throw Exception('Conversation not found');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
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

  Future<Message> sendMessage(int conversationId, String content) async {
    try {
      final request = SendMessageRequest(content: content);
      final response = await _apiCore.post(
        ApiUrls().messagesByConversationId(conversationId),
        request,
      );

      switch (response.statusCode) {
        case 200:
        case 201:
          {
            final responseModel = MessageResponse(response: response);
            responseModel.fromJson();
            return responseModel.message;
          }
        case 401:
          throw Exception('Unauthorized');
        case 403:
          throw Exception('Forbidden - not your conversation');
        case 404:
          throw Exception('Conversation not found');
        case 409:
          throw Exception('Conversation is closed');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }
}

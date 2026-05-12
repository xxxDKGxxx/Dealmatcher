import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/conversation.dart';

class ConversationListResponse extends ResponseModel {
  ConversationListResponse({required super.response});

  final List<ConversationDetail> conversations = [];

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    for (var dataConversation in data) {
      final conversation = ConversationDetail.fromJson(dataConversation);
      conversations.add(conversation);
    }
  }
}

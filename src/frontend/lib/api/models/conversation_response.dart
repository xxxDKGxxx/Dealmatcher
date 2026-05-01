import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/conversation.dart';

class ConversationResponse extends ResponseModel {
  ConversationResponse({required super.response});

  late ConversationDetail conversationDetail;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    conversationDetail = ConversationDetail.fromJson(data);
  }
}

import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class ConversationCreateRequest extends RequestModel {
  ConversationCreateRequest({
    required this.offerId,
    required this.initialMessage,
  });

  final int offerId;
  final String initialMessage;

  @override
  String toJson() {
    final data = {'offerId': offerId, 'initialMessage': initialMessage};
    return jsonEncode(data);
  }
}

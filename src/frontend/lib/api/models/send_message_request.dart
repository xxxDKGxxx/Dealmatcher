import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class SendMessageRequest extends RequestModel {
  SendMessageRequest({required this.content});

  final String content;

  @override
  String toJson() {
    final data = {'content': content};
    return jsonEncode(data);
  }
}

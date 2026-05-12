import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/message.dart';

class MessageResponse extends ResponseModel {
  MessageResponse({required super.response});

  late Message message;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);

    try {
      message = Message.fromJson(data as Map<String, dynamic>);
    } catch (e) {
      throw Exception('Message response does not contain valid data.');
    }
  }
}

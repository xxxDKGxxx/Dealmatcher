import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';

class AuthResponse extends ResponseModel {
  AuthResponse({required super.response});

  late String token;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    if (data.containsKey('accessToken') && data['accessToken'] != null) {
      token = data['accessToken'];
    } else {
      throw Exception(
        'Authentication response does not contain valid accessToken',
      );
    }
  }
}

import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class AuthRequest extends RequestModel {
  const AuthRequest({required this.email, required this.password});

  final String email;
  final String password;

  @override
  String toJson() {
    return jsonEncode({'email': email, 'password': password});
  }
}

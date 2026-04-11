import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class RegisterRequest extends RequestModel {
  const RegisterRequest({
    required this.email,
    required this.password,
    required this.name,
    required this.surname,
  });

  final String email;
  final String password;
  final String name;
  final String surname;

  @override
  String toJson() {
    return jsonEncode({
      'email': email,
      'password': password,
      'name': name,
      'surname': surname,
    });
  }
}

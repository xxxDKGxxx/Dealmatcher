import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class RegisterRequest extends RequestModel {
  const RegisterRequest({
    required this.email,
    required this.password,
    required this.name,
    required this.surname,
    this.login,
    this.birthday,
    this.companyName,
  });

  final String email;
  final String password;
  final String name;
  final String surname;

  // unused properties present in register form
  final DateTime? birthday;
  final String? login;
  final String? companyName;

  @override
  String toJson() {
    return jsonEncode({
      'email': email,
      'password': password,
      'name': name,
      'surname': surname,

      // unused properties present in register form
      'birthday': birthday?.toIso8601String(),
      'login': login,
      'companyName': companyName,
    });
  }
}

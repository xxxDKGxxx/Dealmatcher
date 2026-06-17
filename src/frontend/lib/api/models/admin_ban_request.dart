import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class AdminBanRequest extends RequestModel {
  const AdminBanRequest({
    required this.userId,
    required this.reason,
    required this.expiresAt,
  });

  final int userId;
  final String reason;
  final DateTime expiresAt;

  @override
  String toJson() {
    return jsonEncode({
      'userId': userId,
      'reason': reason,
      'expiresAt': expiresAt.toIso8601String(),
    });
  }
}

import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/ban.dart';

class AdminGetBansResponse extends ResponseModel {
  final List<Ban> bans = [];

  AdminGetBansResponse({required super.response});

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    for (var ban in data) {
      bans.add(Ban.fromJson(ban));
    }
  }
}

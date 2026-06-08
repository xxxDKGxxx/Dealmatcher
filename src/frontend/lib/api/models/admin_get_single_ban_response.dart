import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/ban.dart';

class AdminGetSingleBanResponse extends ResponseModel {
  late Ban ban;

  AdminGetSingleBanResponse({required super.response});

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    ban = Ban.fromJson(data);
  }
}

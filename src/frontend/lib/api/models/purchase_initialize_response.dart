import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';

class PurchaseInitializeResponse extends ResponseModel {
  PurchaseInitializeResponse({required super.response});

  late String redirectUrl;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    redirectUrl = data['redirectUrl'];
  }
}

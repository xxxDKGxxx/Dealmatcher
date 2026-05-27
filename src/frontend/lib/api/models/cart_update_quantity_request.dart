import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class CartUpdateQuantityRequest extends RequestModel {
  const CartUpdateQuantityRequest({required this.quantity});

  final int quantity;

  @override
  String toJson() {
    final data = {"quantity": quantity};
    return jsonEncode(data);
  }
}

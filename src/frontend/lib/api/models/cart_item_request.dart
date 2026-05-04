import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class CartItemRequest extends RequestModel {
  const CartItemRequest({required this.offerId, this.quantity = 1});

  final int offerId;
  final int quantity;

  @override
  String toJson() {
    final data = {
      "offerId": offerId,
      "quantity": quantity,
    };
    return jsonEncode(data);
  }
}
import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/cart_item.dart';

class CartGetItemsResponse extends ResponseModel {
  CartGetItemsResponse({required super.response});

  final List<CartItem> cartItems = [];

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    for (var item in data) {
      final cartItem = CartItem.fromJson(item);
      cartItems.add(cartItem);
    }
  }
}

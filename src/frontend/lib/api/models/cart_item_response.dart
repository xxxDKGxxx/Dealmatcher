import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/cart_item.dart';

class CartItemResponse extends ResponseModel {
  CartItemResponse({required super.response});

  late CartItem cartItem;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    cartItem = CartItem.fromJson(data);
  }
}

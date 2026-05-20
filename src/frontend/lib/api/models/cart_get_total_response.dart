import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/price.dart';

class CartGetTotalResponse extends ResponseModel {
  CartGetTotalResponse({required super.response});

  late Price price;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);

    if (data['totalPrice'] == null || data['currency'] == null) {
      return;
    }

    final value = data['totalPrice'].toDouble();

    if (value == null){
      return;
    }

    price = Price(value: value, currency: data['currency']);
  }
}

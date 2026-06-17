import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/payment_method.dart';

class PaymentMethodsResponse extends ResponseModel {
  PaymentMethodsResponse({required super.response});

  late List<PaymentMethod> paymentMethods;

  @override
  void fromJson() {
    paymentMethods = [];

    final data = jsonDecode(response.body);

    for (var item in data) {
      try {
        paymentMethods.add(
          PaymentMethod.fromJson(item as Map<String, dynamic>),
        );
      } catch (e) {
        throw Exception(
          'Payment methods response does not contain valid data.',
        );
      }
    }
  }
}

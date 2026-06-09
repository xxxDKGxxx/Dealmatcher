import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class PurchaseInitializeRequest extends RequestModel {
  const PurchaseInitializeRequest({
    required this.offerId,
    required this.deliveryMethodId,
    required this.paymentMethodId,
    required this.quantity,
  });

  final int offerId;
  final String deliveryMethodId;
  final String paymentMethodId;
  final int quantity;

  @override
  String toJson() {
    return jsonEncode({
      'offerId': offerId,
      'deliveryMethodId': deliveryMethodId,
      'paymentMethodId': paymentMethodId,
      'quantity': quantity,
    });
  }
}

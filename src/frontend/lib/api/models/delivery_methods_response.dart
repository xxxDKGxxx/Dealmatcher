import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/delivery_method.dart';

class DeliveryMethodsResponse extends ResponseModel {
  DeliveryMethodsResponse({required super.response});

  late List<DeliveryMethod> deliveryMethods;

  @override
  void fromJson() {
    deliveryMethods = [];

    final data = jsonDecode(response.body);

    for (var item in data) {
      try {
        deliveryMethods.add(
          DeliveryMethod.fromJson(item as Map<String, dynamic>),
        );
      } catch (e) {
        throw Exception(
          'Delivery methods response does not contain valid data.',
        );
      }
    }
  }
}

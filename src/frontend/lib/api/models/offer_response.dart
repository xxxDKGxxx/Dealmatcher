import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/offer.dart';

class OfferResponse extends ResponseModel {
  OfferResponse({required super.response});

  late Offer offer;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    try {
      offer = Offer.fromJson(data as Map<String, dynamic>);
    } catch (e) {
      throw Exception('Offer response does not contain valid data.');
    }
  }
}

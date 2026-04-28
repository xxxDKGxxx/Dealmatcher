import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/offer.dart';

class OfferSearchResponse extends ResponseModel {
  late List<Offer> offers = [];

  OfferSearchResponse({required super.response});

  @override
  void fromJson() {
    final List<dynamic> dataList = jsonDecode(response.body);

    for (var data in dataList) {
      try {
        offers.add(Offer.fromJson(data as Map<String, dynamic>));
      } catch (e) {
        throw Exception('Offer search response does not contain valid data.');
      }
    }
  }
}

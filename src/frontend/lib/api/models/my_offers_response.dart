import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/offer.dart';

class MyOffersResponse extends ResponseModel {
  MyOffersResponse({required super.response});

  List<Offer> offers = [];

  @override
  void fromJson() {
    offers.clear();
    final dataList = jsonDecode(response.body);

    for (var data in dataList) {
      try {
        offers.add(Offer.fromJson(data as Map<String, dynamic>));
      } catch (e) {
        throw Exception('My Offers response does not contain valid data.');
      }
    }
  }
}

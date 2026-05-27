import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/all_offers.dart';

class AdminAllOffersResponse extends ResponseModel {
  late AllOffers allOffers;

  AdminAllOffersResponse({required super.response});

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    allOffers = AllOffers.fromJson(data);
  }
}

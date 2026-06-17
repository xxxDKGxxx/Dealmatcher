import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/activity.dart';

class AdminOfferActivityResponse extends ResponseModel {
  AdminOfferActivityResponse({required super.response});

  final List<Activity> activities = [];

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    for (var item in data) {
      try {
        final activity = Activity.fromJson(item as Map<String, dynamic>);
        activities.add(activity);
      } catch (e) {
        throw Exception('Offer activity response does not contain valid data.');
      }
    }
  }
}

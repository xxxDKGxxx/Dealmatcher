import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class OfferUpdateRequest extends RequestModel {
  const OfferUpdateRequest({
    required this.title,
    required this.description,
    required this.price,
    required this.images,
    required this.tags,
    required this.properties,
    required this.availability,
  });

  final String? title;
  final String? description;
  final double? price;
  final List<String>? images;
  final List<String>? tags;
  final Map<String, String>? properties;
  final int? availability;

  @override
  String toJson() {
    return jsonEncode({
      'title': title,
      'description': description,
      'price': price,
      'images': images,
      'tags': tags,
      'properties': properties,
      'availability': availability,
    });
  }
}

import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class OfferSearchRequest extends RequestModel {
  const OfferSearchRequest({
    required this.categoryId,
    required this.minPrice,
    required this.maxPrice,
    required this.tags,
    this.properties = const {},
    this.searchPhrase = '',
    this.limit = 20,
  });

  final int? categoryId;
  final double minPrice;
  final double maxPrice;
  final List<String> tags;
  final Map<String, List<String>> properties;
  final String searchPhrase;
  final int limit;

  @override
  String toJson() {
    return jsonEncode({
      'categoryId': categoryId,
      'minPrice': minPrice,
      'maxPrice': maxPrice,
      'tags': tags,
      'properties': properties,
      'searchPhrase': searchPhrase,
      'limit': limit,
    });
  }
}

import 'package:frontend/models/property_definition.dart';

class OfferSearchRequest {
  const OfferSearchRequest({
    required this.categoryId,
    required this.minPrice,
    required this.maxPrice,
    required this.tags,
    this.properties = const [],
    this.searchPhrase = '',
    this.limit = 20,
  });

  final int categoryId;
  final double minPrice;
  final double maxPrice;
  final List<String> tags;
  final List<PropertyDefinition> properties;
  final String searchPhrase;
  final int limit;
}

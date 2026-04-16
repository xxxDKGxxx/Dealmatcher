import 'category.dart';

class Offer {
  const Offer({
    required this.id,
    required this.title,
    required this.description,
    required this.price,
    required this.images,
    required this.seller,
    required this.category,
    required this.tags,
    required this.properties,
    required this.availability,
    required this.status,
    required this.createdAt,
    required this.updatedAt,
  });

  final int id;
  final String title;
  final String description;
  final double price;
  final List<String> images;
  final Seller seller;
  final Category category;
  final List<String> tags;
  final List<OfferProperty> properties;
  final int availability;
  final OfferStatus status;
  final DateTime createdAt;
  final DateTime updatedAt;
}

class Seller {
  const Seller({required this.id, required this.name});
  final int id;
  final String name;
}

class OfferProperty {
  const OfferProperty({
    required this.id,
    required this.name,
    required this.value,
  });
  final int id;
  final String name;
  final String value;
}

enum OfferStatus {
  active,
  deleted,
  sold;

  static OfferStatus fromString(String s) {
    return OfferStatus.values.firstWhere(
      (e) => e.name.toLowerCase() == s.toLowerCase(),
      orElse: () => OfferStatus.active,
    );
  }
}

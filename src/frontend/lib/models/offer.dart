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
  final Map<int, String> properties;
  final int availability;
  final OfferStatus status;
  final DateTime createdAt;
  final DateTime updatedAt;

  factory Offer.fromJson(Map<String, dynamic> json) {
    final List<String> parsedImages = [];
    if (json['images'] != null) {
      for (var image in json['images']) {
        parsedImages.add(image.toString());
      }
    }

    final List<String> parsedTags = [];
    if (json['tags'] != null) {
      for (var tag in json['tags']) {
        parsedTags.add(tag.toString());
      }
    }

    final Map<int, String> parsedProperties = {};
    if (json['properties'] != null) {
      final props = json['properties'] as Map<String, dynamic>;
      props.forEach((key, value) {
        final intKey = int.tryParse(key);
        if (intKey != null) {
          parsedProperties[intKey] = value.toString();
        }
      });
    }

    return Offer(
      id: json['id'] as int,
      title: json['title'] as String,
      description: json['description'] as String,
      price: (json['price'] as num).toDouble(),
      images: parsedImages,
      seller: Seller.fromJson(json['seller'] as Map<String, dynamic>),
      category: Category.fromJson(json['category'] as Map<String, dynamic>),
      tags: parsedTags,
      properties: parsedProperties,
      availability: json['availability'] as int,
      status: OfferStatus.fromString(json['status'] as String),
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: DateTime.parse(json['updatedAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'description': description,
      'price': price,
      'images': images,
      'seller': seller.toJson(),
      'category': category.toJson(),
      'tags': tags,
      'properties': properties.map((key, value) => MapEntry(key.toString(), value)),
      'availability': availability,
      'status': status.name,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }
}

class Seller {
  const Seller({required this.id, required this.name});
  final int id;
  final String name;

  factory Seller.fromJson(Map<String, dynamic> json) {
    return Seller(
      id: json['id'] as int,
      name: json['name'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
    };
  }
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
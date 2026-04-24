import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';

class OfferResponse extends ResponseModel {
  OfferResponse({required super.response});

  late Offer offer;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);

    bool checkData(dynamic data, String key) {
      return data.containsKey(key) && data[key] != null;
    }

    if (checkData(data, 'id') &&
        checkData(data, 'title') &&
        checkData(data, 'description') &&
        checkData(data, 'price') &&
        checkData(data, 'images') &&
        checkData(data, 'seller') &&
        checkData(data, 'category') &&
        checkData(data, 'tags') &&
        checkData(data, 'properties') &&
        checkData(data, 'availability') &&
        checkData(data, 'status') &&
        checkData(data, 'createdAt') &&
        checkData(data, 'updatedAt') &&
        // seller
        checkData(data['seller'], 'id') &&
        checkData(data['seller'], 'name') &&
        // category
        checkData(data['category'], 'id') &&
        checkData(data['category'], 'name') &&
        checkData(data['category'], 'description')) {
      final List<String> images = [];
      for (var image in data['images']) {
        images.add(image.toString());
      }

      final seller = Seller(
        id: data['seller']['id'] as int,
        name: data['seller']['name'] as String,
      );

      final category = Category(
        id: data['category']['id'] as int,
        name: data['category']['name'] as String,
        description: data['category']['description'] as String,
      );

      final List<String> tags = [];
      for (var tag in data['tags']) {
        tags.add(tag.toString());
      }

      final Map<int, String> properties = {};
      final props = data['properties'] as Map<String, dynamic>;
      props.forEach((key, value) {
        final intKey = int.tryParse(key);
        if (intKey != null) {
          properties[intKey] = value.toString();
        }
      });

      offer = Offer(
        id: data['id'] as int,
        title: data['title'] as String,
        description: data['description'] as String,
        price: (data['price'] as num).toDouble(),
        images: images,
        seller: seller,
        category: category,
        tags: tags,
        properties: properties,
        availability: data['availability'] as int,
        status: OfferStatus.fromString(data['status'] as String),
        createdAt: DateTime.parse(data['createdAt'] as String),
        updatedAt: DateTime.parse(data['updatedAt'] as String),
      );
    }
  }
}

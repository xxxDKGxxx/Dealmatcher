import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';

class MyOffersResponse extends ResponseModel {
  MyOffersResponse({required super.response});

  List<Offer> offers = [];

  @override
  void fromJson() {
    offers.clear();
    final dataList = jsonDecode(response.body);

    for (var data in dataList) {
      final List<String> images = [];
      if (data['images'] != null) {
        for (var image in data['images']) {
          images.add(image.toString());
        }
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
      if (data['tags'] != null) {
        for (var tag in data['tags']) {
          tags.add(tag.toString());
        }
      }

      final Map<int, String> properties = {};
      if (data['properties'] != null) {
        final props = data['properties'] as Map<String, dynamic>;
        props.forEach((key, value) {
          final intKey = int.tryParse(key);
          if (intKey != null) {
            properties[intKey] = value.toString();
          }
        });
      }

      final offer = Offer(
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

      offers.add(offer);
    }
  }
}

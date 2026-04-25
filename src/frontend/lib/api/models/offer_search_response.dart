import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';

class OfferSearchResponse extends ResponseModel {
  late List<Offer> offers = [];

  OfferSearchResponse({required super.response});

  @override
  void fromJson() {
    final List<dynamic> dataList = jsonDecode(response.body);

    for (var data in dataList) {
      bool checkData(dynamic data, String key) {
        return data.containsKey(key) && data[key] != null;
      }

      if (checkData(data, 'id') &&
          checkData(data, 'title') &&
          checkData(data, 'description') &&
          checkData(data, 'price') &&
          checkData(data, 'category') &&
          checkData(data, 'seller')) {
        final List<String> images = [];
        if (checkData(data, 'images')) {
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
        if (checkData(data, 'tags')) {
          for (var tag in data['tags']) {
            tags.add(tag.toString());
          }
        }

        final Map<int, String> properties = {};
        if (checkData(data, 'properties')) {
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
}

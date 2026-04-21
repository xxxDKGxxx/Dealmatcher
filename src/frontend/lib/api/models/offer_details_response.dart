import 'dart:convert';
import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';

class OfferDetailsResponse extends ResponseModel {
  OfferDetailsResponse({required super.response});

  late Offer? offer;

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
      // images
      List<String> images = [];
      for (var image in data['images']) {
        images.add(image);
      }

      // seller
      dynamic dataSeller = data['seller'];
      Seller seller = Seller(id: dataSeller['id'], name: dataSeller['name']);

      // category
      dynamic dataCategory = data['category'];
      Category category = Category(
        id: dataCategory['id'],
        name: dataCategory['name'],
        description: dataCategory['description'],
      );

      // tags
      List<String> tags = [];
      for (var tag in data['tags']) {
        tags.add(tag);
      }

      // properties
      dynamic dataProperties = data['properties'];
      Map<int, String> properties = {};
      for (var i = 0; i < dataProperties.length; i++) {
        properties[i] = dataProperties[i];
      }

      // status
      OfferStatus status = OfferStatus.fromString(data['status']);

      // created and updated at
      DateTime createdAt = DateTime.parse(data['createdAt']);
      DateTime updatedAt = DateTime.parse(data['updatedAt']);

      offer = Offer(
        id: data['id'],
        title: data['title'],
        description: data['description'],
        price: data['price'],
        images: images,
        seller: seller,
        category: category,
        tags: tags,
        properties: properties,
        availability: data['availability'],
        status: status,
        createdAt: createdAt,
        updatedAt: updatedAt,
      );
    } else {
      throw Exception(
        'Offer details response does not contain valid offer data.',
      );
    }
  }
}

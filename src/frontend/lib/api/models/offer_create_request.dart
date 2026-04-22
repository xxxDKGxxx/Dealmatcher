import 'dart:convert';

import 'package:frontend/api/models/multipart_request_model.dart';
import 'package:image_picker/image_picker.dart';

class OfferCreateRequest extends MultipartRequestModel {
  const OfferCreateRequest({
    required this.title,
    required this.description,
    required this.price,
    required this.images,
    required this.categoryId,
    required this.tags,
    required this.properties,
    required this.availability,
  });

  final String title;
  final String description;
  final double price;
  final List<XFile> images;
  final int categoryId;
  final List<String> tags;
  final Map<String, String> properties;
  final int availability;

  @override
  toMultipartFields() {
    return {
      'title': title,
      'description': description,
      'price': price.toString(), // Rzutowanie na string!
      'categoryId': categoryId.toString(), // Rzutowanie na string!
      'availability': availability.toString(), // Rzutowanie na string!
      'tags': jsonEncode(tags),
      'properties': jsonEncode(properties),
    };
  }
}

import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/category.dart';

class CategoriesResponse extends ResponseModel {
  CategoriesResponse({required super.response});

  late List<Category> categories;

  @override
  void fromJson() {
    categories = [];

    final data = jsonDecode(response.body);

    for (var item in data) {
      if (item.containsKey('id') &&
          item['id'] != null &&
          item.containsKey('name') &&
          item['name'] != null &&
          item.containsKey('description') &&
          item['description'] != null) {
        var category = Category(
          id: item['id'],
          name: item['name'],
          description: item['description'],
        );
        categories.add(category);
      } else {
        throw Exception('Category response does not contain valid data.');
      }
    }
  }
}

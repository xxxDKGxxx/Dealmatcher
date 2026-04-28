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
      try {
        categories.add(Category.fromJson(item as Map<String, dynamic>));
      } catch (e) {
        throw Exception('Category response does not contain valid data.');
      }
    }
  }
}

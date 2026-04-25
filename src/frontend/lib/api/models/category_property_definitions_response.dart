import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/property_definition.dart';

class CategoryPropertyDefinitionsResponse extends ResponseModel {
  CategoryPropertyDefinitionsResponse({required super.response});

  late List<PropertyDefinition> propertyDefinitions;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);

    propertyDefinitions = [];

    for (var item in data) {
      try {
        propertyDefinitions.add(
          PropertyDefinition.fromJson(item as Map<String, dynamic>),
        );
      } catch (e) {
        throw Exception(
          'Property definition response does not contain valid data.',
        );
      }
    }
  }
}

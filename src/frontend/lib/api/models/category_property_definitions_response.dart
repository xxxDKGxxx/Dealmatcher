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
      if (item.containsKey('id') &&
          item['id'] != null &&
          item.containsKey('name') &&
          item['name'] != null &&
          item.containsKey('type') &&
          item['type'] != null &&
          item.containsKey('options')) {
        var propertyDefinition = PropertyDefinition(
          id: item['id'],
          name: item['name'],
          type: PropertyType.fromString(item['type']),
          options: [],
        );

        if (item['options'] != null) {
          for (var option in item['options']) {
            propertyDefinition.options.add(option);
          }
        }
        propertyDefinitions.add(propertyDefinition);
      } else {
        throw Exception(
          'Property definition response does not contain valid data.',
        );
      }
    }
  }
}

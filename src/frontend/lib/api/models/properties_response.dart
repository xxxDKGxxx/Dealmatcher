import 'dart:convert';
import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/property_definition.dart';

class PropertiesResponse extends ResponseModel {
  PropertiesResponse({required super.response});

  List<PropertyDefinition> properties = [];

  @override
  void fromJson() {
    final data = jsonDecode(response.body);

    bool checkData(dynamic data, String key) {
      return data.containsKey(key) && data[key] != null;
    }

    for (var dataProperty in data) {
      if (checkData(dataProperty, 'id') &&
          checkData(dataProperty, 'name') &&
          checkData(dataProperty, 'type') &&
          checkData(dataProperty, 'options')) {
        List<String> options = [];
        for (var option in dataProperty['options']) {
          options.add(option);
        }
        PropertyDefinition propertyDefinition = PropertyDefinition(
          id: dataProperty['id'],
          name: dataProperty['name'],
          type: dataProperty['type'],
          options: options,
        );
        properties.add(propertyDefinition);
      } else {
        throw Exception(
          'One or more of properties in response do not contain valid property data.',
        );
      }
    }
  }
}

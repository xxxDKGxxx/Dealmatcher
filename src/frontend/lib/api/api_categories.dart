import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/categoreis_response.dart';
import 'package:frontend/api/models/category_property_definitions_response.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/property_definition.dart';

class ApiCategories {
  final ApiCore _apiCore = ApiCore();

  Future<List<Category>> getCategories() async {
    late List<Category> categories;
    try {
      final response = await _apiCore.get(ApiUrls().categories);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = CategoriesResponse(response: response);
            responseModel.fromJson();
            categories = responseModel.categories;
          }
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return categories;
  }

  Future<List<PropertyDefinition>> getPropertyDefinitions(
    String categoryName,
  ) async {
    late List<PropertyDefinition> propertyDefinitions;
    try {
      final response = await _apiCore.get(
        ApiUrls().propertiesByCategoryName(categoryName),
      );

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = CategoryPropertyDefinitionsResponse(
              response: response,
            );
            responseModel.fromJson();
            propertyDefinitions = responseModel.propertyDefinitions;
          }
        case 404:
          throw Exception('Category not found');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return propertyDefinitions;
  }
}

import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/properties_request.dart';
import 'package:frontend/api/models/properties_response.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/property_definition.dart';

class ApiCategoriesAndProperties {
  final ApiCore _apiCore = ApiCore();

  // blank since not needed now -- will populate with appropriate code later
  Future<List<Category>> getCategories() async {
    return [];
  }

  Future<List<PropertyDefinition>> getProperties(Category category) async {
    List<PropertyDefinition> properties = [];
    try {
      final request = PropertiesRequest(categoryName: category.name);
      final response = await _apiCore.post(
        ApiUrls().properties(category.id),
        request,
      );

      switch (response.statusCode) {
        case 200:
          {
            final responseMode = PropertiesResponse(response: response);
            responseMode.fromJson();
            properties.addAll(responseMode.properties);
          }
        case 404:
          throw Exception('Category not found.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown error: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return properties;
  }
}

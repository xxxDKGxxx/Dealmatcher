import 'package:frontend/api/models/request_model.dart';

class PropertiesRequest extends RequestModel {
  const PropertiesRequest({required this.categoryName});

  final String categoryName;

  @override
  String toJson() {
    return categoryName;
  }
}

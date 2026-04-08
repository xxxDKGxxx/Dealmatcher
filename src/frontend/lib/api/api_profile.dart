import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/profile_response.dart';
import 'package:frontend/models/user.dart';

class ApiProfile {
  final ApiCore _apiCore = ApiCore();
  final String _apiLoginUrl = ApiUrls().profile;

  Future<User> getProfile() async {
    late User user;
    try {
      final response = await _apiCore.get(_apiLoginUrl);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = ProfileResponse(response: response);
            responseModel.fromJson();
            user = responseModel.user;
          }
        case 401:
          throw Exception('Invalid credentials.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return user;
  }
}

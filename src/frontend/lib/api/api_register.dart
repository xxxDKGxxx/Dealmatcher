import 'package:frontend/api/api_auth.dart';
import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/register_request.dart';

class ApiRegister {
  final _apiCore = ApiCore();
  final _apiRegisterUrl = ApiUrls().register;

  Future<void> register(
    String email,
    String password,
    String name,
    String surname,
  ) async {
    try {
      final request = RegisterRequest(
        email: email,
        password: password,
        name: name,
        surname: surname,
      );
      final response = await _apiCore.post(_apiRegisterUrl, request);

      switch (response.statusCode) {
        case 200:
        case 201:
          {
            await ApiAuth().login(email, password);
          }
        case 400:
          throw Exception('Invalid registration data');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown error: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }
}

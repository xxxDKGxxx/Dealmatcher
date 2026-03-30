import 'dart:convert';
import 'api_core.dart';

class ApiAuth {
  final ApiCore _apiCore = ApiCore();

  Future<void> login(String email, String password) async {
    try {
      final response = await _apiCore.post('/users/login', {
        'email': email,
        'password': password,
      });

      switch (response.statusCode) {
        case 200:
          {
            final data = jsonDecode(response.body);
            _apiCore.setToken(data['accessToken']);
            return data;
          }
        case 401:
          throw Exception('Invalid credentials.');
        case 403:
          throw Exception('User is banned.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown error: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  void logout() {
    _apiCore.setToken(null);
  }
}

import 'dart:convert';
import 'package:frontend/api/api_urls.dart';

import 'api_core.dart';

class ApiAuth {
  final ApiCore _apiCore = ApiCore();
  final String _apiLoginUrl = ApiUrls().login;

  Future<void> login(String email, String password) async {
    try {
      final response = await _apiCore.post(_apiLoginUrl, {
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
        case 404:
          throw Exception('User does not exist.');
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

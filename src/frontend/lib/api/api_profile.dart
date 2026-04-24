import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/my_offers_response.dart';
import 'package:frontend/api/models/profile_edit_request.dart';
import 'package:frontend/api/models/profile_response.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/user.dart';

class ApiProfile {
  final ApiCore _apiCore = ApiCore();
  final String _apiProfileUrl = ApiUrls().profile;

  Future<User> getProfile() async {
    late User user;
    try {
      final response = await _apiCore.get(_apiProfileUrl);

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

  Future<void> updateProfile(String name, String surname) async {
    try {
      final request = ProfileEditRequest(name: name, surname: surname);
      final response = await _apiCore.put(_apiProfileUrl, request);

      switch (response.statusCode) {
        case 200:
          {}
        case 400:
          throw Exception('Invalid update data.');
        case 401:
          throw Exception('Unauthorized');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<List<Offer>> getProfileOffers() async {
    try {
      final response = await _apiCore.get(ApiUrls().myOffers);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = MyOffersResponse(response: response);
            responseModel.fromJson();
            return responseModel.offers;
          }
        case 204:
          throw Exception('No offers found.');
        case 401:
          throw Exception('Unauthorized.');
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

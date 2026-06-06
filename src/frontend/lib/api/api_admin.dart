import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/admin_all_offers_response.dart';
import 'package:frontend/api/models/admin_all_users_response.dart';
import 'package:frontend/api/models/admin_user_activity_response.dart';
import 'package:frontend/models/activity.dart';
import 'package:frontend/models/all_offers.dart';
import 'package:frontend/models/all_users.dart';

class ApiAdmin {
  final ApiCore _apiCore = ApiCore();
  final _getOffersUrl = ApiUrls().adminGetOffers;
  final _getUsersUrl = ApiUrls().adminGetUsers;

  Future<AllOffers> getOffers({
    required int page,
    required int limit,
    required String status,
  }) async {
    try {
      final response = await _apiCore.get(
        '$_getOffersUrl?page=$page&limit=$limit&status=$status',
      );

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = AdminAllOffersResponse(response: response);
            responseModel.fromJson();
            return responseModel.allOffers;
          }
        case 400:
          throw Exception('Invalid request parameters.');
        case 401:
          throw Exception('Unauthorized.');
        case 403:
          throw Exception('Forbidden - admin only.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown error: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<AllUsers> getUsers({
    required int page,
    required int limit,
    required String status,
  }) async {
    try {
      final response = await _apiCore.get(
        '$_getUsersUrl?page=$page&limit=$limit&status=$status',
      );

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = AdminAllUsersResponse(response: response);
            responseModel.fromJson();
            return responseModel.allUsers;
          }
        case 400:
          throw Exception('Invalid request parameters.');
        case 401:
          throw Exception('Unauthorized.');
        case 403:
          throw Exception('Forbidden - admin only.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown error: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<List<Activity>> getUserActivity(
    int userId, {
    DateTime? from,
    DateTime? to,
  }) async {
    try {
      var url = ApiUrls().adminUserActivity(userId);
      final queryParams = <String>[];
      if (from != null) queryParams.add('from=${from.toIso8601String()}');
      if (to != null) queryParams.add('to=${to.toIso8601String()}');
      if (queryParams.isNotEmpty) {
        url += '?${queryParams.join('&')}';
      }

      final response = await _apiCore.get(url);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = AdminUserActivityResponse(response: response);
            responseModel.fromJson();
            return responseModel.activities;
          }
        case 401:
          throw Exception('Unauthorized.');
        case 403:
          throw Exception('Forbidden - admin only.');
        case 404:
          throw Exception('User not found.');
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

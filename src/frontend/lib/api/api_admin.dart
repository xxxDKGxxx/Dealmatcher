import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/admin_all_offers_response.dart';
import 'package:frontend/models/all_offers.dart';

class ApiAdmin {
  final ApiCore _apiCore = ApiCore();
  final _getOffersUrl = ApiUrls().adminGetOffers;

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
}

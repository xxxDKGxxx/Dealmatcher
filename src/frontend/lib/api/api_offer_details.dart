import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/offer_details_request.dart';
import 'package:frontend/api/models/offer_details_response.dart';
import 'package:frontend/models/offer.dart';

class ApiOfferDetails {
  final ApiCore _apiCore = ApiCore();
  final String _apiLoginUrl = ApiUrls().offerDetails;

  Future<Offer?> getOffer(int offerId) async {
    Offer? offer;
    try {
      final request = OfferDetailsRequest(offerId: offerId);
      final response = await _apiCore.post(_apiLoginUrl, request);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = OfferDetailsResponse(response: response);
            responseModel.fromJson();
            offer = responseModel.offer;
          }
        case 404:
          throw Exception('Offer does not exist.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown error: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return offer;
  }
}

import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/offer_details_response.dart';
import 'package:frontend/models/offer.dart';

class ApiOfferDetails {
  final ApiCore _apiCore = ApiCore();

  Future<Offer?> getOffer(int offerId) async {
    Offer? offer;
    try {
      final response = await _apiCore.get(ApiUrls().offerDetails(offerId));

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

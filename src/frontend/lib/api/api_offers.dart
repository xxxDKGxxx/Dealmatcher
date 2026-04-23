import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/my_offers_response.dart';
import 'package:frontend/api/models/offer_create_request.dart';
import 'package:frontend/api/models/offer_response.dart';
import 'package:frontend/models/offer.dart';
import 'package:http/http.dart' as http;

class ApiOffers {
  final ApiCore _apiCore = ApiCore();

  Future<Offer> createOffer(OfferCreateRequest request) async {
    try {
      final fields = request.toMultipartFields();
      final List<http.MultipartFile> files = [];

      for (final image in request.images) {
        files.add(
          http.MultipartFile.fromBytes(
            'images',
            await image.readAsBytes(),
            filename: image.name,
          ),
        );
      }

      final response = await _apiCore.postMultipart(
        ApiUrls().offers,
        fields: fields,
        files: files,
      );

      switch (response.statusCode) {
        case 201:
          {
            final responseModel = OfferResponse(response: response);
            responseModel.fromJson();
            return responseModel.offer;
          }
        case 400:
          throw Exception('Invalid offer data.');
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

  Future<List<Offer>> getMyOffers() async {
    try {
      final response = await _apiCore.get(ApiUrls().myOffers);

      switch(response.statusCode) {
        case 200: {
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

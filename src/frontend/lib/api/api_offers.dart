import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/offer_change_status_request.dart';
import 'package:frontend/api/models/offer_create_request.dart';
import 'package:frontend/api/models/offer_response.dart';
import 'package:frontend/api/models/offer_search_request.dart';
import 'package:frontend/api/models/offer_search_response.dart';
import 'package:frontend/api/models/offer_update_request.dart';
import 'package:frontend/models/offer.dart';
import 'package:http/http.dart' as http;

class ApiOffers {
  final ApiCore _apiCore = ApiCore();

  Future<Offer> changeOfferStatus(
    int offerId,
    OfferChangeStatusRequest request,
  ) async {
    late Offer offer;
    try {
      final response = await _apiCore.put(
        ApiUrls().offerUpdateStatus(offerId),
        request,
      );
      switch (response.statusCode) {
        case 200:
          {
            final responseModel = OfferResponse(response: response);
            responseModel.fromJson();
            offer = responseModel.offer;
          }
        case 400:
          throw Exception('Invalid status value.');
        case 401:
          throw Exception('Invalid credentials.');
        case 403:
          throw Exception(
            'Forbidden - you do not have permissions to edit this offer.',
          );
        case 404:
          throw Exception('Offer not found.');
        case 409:
          throw Exception('Cannot change status from current state.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }

    return offer;
  }

  Future deleteOffer(int offerId) async {
    try {
      final response = await _apiCore.delete(ApiUrls().offerById(offerId));
      switch (response.statusCode) {
        case 204:
          return;
        case 401:
          throw Exception('Invalid credentials.');
        case 403:
          throw Exception(
            'Forbidden - you do not have permissions to delete this offer.',
          );
        case 404:
          throw Exception('Offer not found.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<Offer> updateOffer(int offerId, OfferUpdateRequest request) async {
    late Offer offer;
    try {
      final response = await _apiCore.patch(
        ApiUrls().offerById(offerId),
        request,
      );
      switch (response.statusCode) {
        case 200:
          {
            final responseModel = OfferResponse(response: response);
            responseModel.fromJson();
            offer = responseModel.offer;
          }
        case 401:
          throw Exception('Invalid credentials.');
        case 403:
          throw Exception(
            'Forbidden - you do not have permissions to edit this offer.',
          );
        case 404:
          throw Exception('Offer not found.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }

    return offer;
  }

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

  Future<Offer?> getOffer(int offerId) async {
    Offer? offer;
    try {
      final response = await _apiCore.get(ApiUrls().offerDetailsById(offerId));

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = OfferResponse(response: response);
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

  Future<List<Offer>> searchOffers(OfferSearchRequest request) async {
    late List<Offer> offers = [];

    try {
      final response = await _apiCore.post(ApiUrls().searchOffers, request);

      switch (response.statusCode) {
        case 200:
          {
            final OfferSearchResponse responseModel = OfferSearchResponse(
              response: response,
            );

            responseModel.fromJson();
            offers = responseModel.offers;
          }
        case 204:
          offers = [];
        case 400:
          throw Exception('Invalid search parameters');
        case 500:
          throw Exception('Internal server error.');
      }
    } catch (e) {
      rethrow;
    }

    return offers;
  }
}

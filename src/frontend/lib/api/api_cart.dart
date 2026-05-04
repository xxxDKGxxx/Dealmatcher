import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/cart_item_request.dart';

class ApiCart {
  final _apiCore = ApiCore();
  final _apiAddToCartUrl = ApiUrls().cartItems;

  Future<void> addToCart(int offerId, {int quantity = 1}) async {
    try {
      final request = CartItemRequest(offerId: offerId, quantity: quantity);
      final response = await _apiCore.post(_apiAddToCartUrl, request);

      switch (response.statusCode) {
        case 200:
        case 201:
          {
            return;
          }
        case 400:
          throw Exception('Invalid request');
        case 401:
          throw Exception('Unauthorized');
        case 404:
          throw Exception('Offer not found');
        case 409:
          throw Exception('Item already in cart');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }
}
import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/add_cart_item_request.dart';
import 'package:frontend/api/models/add_cart_item_response.dart';
import 'package:frontend/models/cart_item.dart';

class ApiCart {
  final _apiCore = ApiCore();
  final _apiAddToCartUrl = ApiUrls().cartItems;

  Future<CartItem> addToCart(int offerId, {int quantity = 1}) async {
    try {
      final request = AddCartItemRequest(offerId: offerId, quantity: quantity);
      final response = await _apiCore.post(_apiAddToCartUrl, request);

      switch (response.statusCode) {
        case 201:
          {
            final responseModel = AddCartItemResponse(response: response);
            responseModel.fromJson();
            return responseModel.cartItem;
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

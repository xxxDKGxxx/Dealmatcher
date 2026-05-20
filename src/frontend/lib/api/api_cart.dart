import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/cart_add_item_request.dart';
import 'package:frontend/api/models/cart_item_response.dart';
import 'package:frontend/api/models/cart_get_total_response.dart';
import 'package:frontend/api/models/cart_get_items_response.dart';
import 'package:frontend/api/models/cart_update_quantity_request.dart';
import 'package:frontend/models/cart_item.dart';
import 'package:frontend/models/price.dart';

class ApiCart {
  final _apiCore = ApiCore();
  final _apiAddToCartUrl = ApiUrls().cartItems;
  final _apiGetCart = ApiUrls().cartItems;
  final _apiCartTotal = ApiUrls().cartTotal;

  Future<CartItem> addToCart(int offerId, {int quantity = 1}) async {
    try {
      final request = CartAddItemRequest(offerId: offerId, quantity: quantity);
      final response = await _apiCore.post(_apiAddToCartUrl, request);

      switch (response.statusCode) {
        case 201:
          {
            final responseModel = CartItemResponse(response: response);
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

  Future<List<CartItem>> getCart() async {
    try {
      final response = await _apiCore.get(_apiGetCart);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = CartGetItemsResponse(response: response);
            responseModel.fromJson();
            return responseModel.cartItems;
          }
        case 401:
          throw Exception('Unauthorized');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<Price> getCartTotal() async {
    try {
      final response = await _apiCore.get(_apiCartTotal);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = CartGetTotalResponse(response: response);
            responseModel.fromJson();
            return responseModel.price;
          }
        case 401:
          throw Exception('Unauthorized');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<CartItem> updateItemQuantity(int itemId, int quantity) async {
    try {
      final request = CartUpdateQuantityRequest(quantity: quantity);
      final response = await _apiCore.patch(
        ApiUrls().cartItemById(itemId),
        request,
      );

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = CartItemResponse(response: response);
            responseModel.fromJson();
            return responseModel.cartItem;
          }
        case 400:
          throw Exception('Invalid quantity');
        case 401:
          throw Exception('Unauthorized');
        case 403:
          throw Exception('Forbidden - not your cart item');
        case 404:
          throw Exception('Cart item not found');
        case 500:
          throw Exception('Internal server error');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }

  Future<void> removeItem(int itemId) async {
    try {
      final response = await _apiCore.delete(ApiUrls().cartItemById(itemId));

      switch (response.statusCode) {
        case 204: { }
        case 401:
          throw Exception('Unauthorized');
        case 403:
          throw Exception('Forbidden - not your cart item');
        case 404:
          throw Exception('Cart item not found');
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

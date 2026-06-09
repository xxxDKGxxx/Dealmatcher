import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/delivery_methods_response.dart';
import 'package:frontend/api/models/payment_methods_response.dart';
import 'package:frontend/api/models/purchase_initialize_request.dart';
import 'package:frontend/api/models/purchase_initialize_response.dart';
import 'package:frontend/models/delivery_method.dart';
import 'package:frontend/models/payment_method.dart';

class ApiPurchases {
  static final ApiPurchases _instance = ApiPurchases._internal();
  factory ApiPurchases() => _instance;
  ApiPurchases._internal();

  final ApiCore _apiCore = ApiCore();

  Future<List<DeliveryMethod>> getDeliveryMethods() async {
    late List<DeliveryMethod> deliveryMethods;
    try {
      final response = await _apiCore.get(ApiUrls().deliveryMethods);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = DeliveryMethodsResponse(response: response);
            responseModel.fromJson();
            deliveryMethods = responseModel.deliveryMethods;
          }
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return deliveryMethods;
  }

  Future<List<PaymentMethod>> getPaymentMethods() async {
    late List<PaymentMethod> paymentMethods;
    try {
      final response = await _apiCore.get(ApiUrls().paymentMethods);

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = PaymentMethodsResponse(response: response);
            responseModel.fromJson();
            paymentMethods = responseModel.paymentMethods;
          }
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
    return paymentMethods;
  }

  Future<String> initializePurchase(
    int offerId,
    String deliveryMethodId,
    String paymentMethodId,
    int quantity,
  ) async {
    try {
      final request = PurchaseInitializeRequest(
        offerId: offerId,
        deliveryMethodId: deliveryMethodId,
        paymentMethodId: paymentMethodId,
        quantity: quantity,
      );
      final response = await _apiCore.post(
        ApiUrls().initializePurchase,
        request,
      );

      switch (response.statusCode) {
        case 200:
          {
            final responseModel = PurchaseInitializeResponse(
              response: response,
            );
            responseModel.fromJson();
            return responseModel.redirectUrl;
          }
        case 400:
          throw Exception('Invalid purchase data.');
        case 401:
          throw Exception('Unauthorized.');
        case 404:
          throw Exception('Offer not found.');
        case 409:
          throw Exception('Offer not available for purchase.');
        case 500:
          throw Exception('Internal server error.');
        default:
          throw Exception('Unknown server response: ${response.statusCode}');
      }
    } catch (e) {
      rethrow;
    }
  }
}

import 'package:frontend/api/api_core.dart';
import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/delivery_methods_response.dart';
import 'package:frontend/api/models/payment_methods_response.dart';
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
}

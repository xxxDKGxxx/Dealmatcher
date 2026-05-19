import 'package:frontend/models/delivery_method.dart';
import 'package:frontend/models/payment_method.dart';

class ApiPurchases {
  static final ApiPurchases _instance = ApiPurchases._internal();
  factory ApiPurchases() => _instance;
  ApiPurchases._internal();

  // Mocked endpoint for /purchases/delivery-methods
  Future<List<DeliveryMethod>> getDeliveryMethods() async {
    // Simulate network delay
    await Future.delayed(const Duration(milliseconds: 500));

    return [
      DeliveryMethod(
        id: 'del_1',
        name: 'Standard Delivery',
        description: 'Delivery via standard post',
        price: 5.99,
        estimatedDays: 3,
      ),
      DeliveryMethod(
        id: 'del_2',
        name: 'Express Delivery',
        description: 'Next day delivery',
        price: 15.99,
        estimatedDays: 1,
      ),
      DeliveryMethod(
        id: 'del_3',
        name: 'Pickup Point',
        description: 'Deliver to a local pickup point',
        price: 3.99,
        estimatedDays: 2,
      ),
    ];
  }

  // Mocked endpoint for /purchases/payment-methods
  Future<List<PaymentMethod>> getPaymentMethods() async {
    // Simulate network delay
    await Future.delayed(const Duration(milliseconds: 500));

    return [
      PaymentMethod(
        id: 'pay_1',
        name: 'Credit Card',
        provider: 'Stripe',
        icon: 'credit_card',
      ),
      PaymentMethod(
        id: 'pay_2',
        name: 'PayPal',
        provider: 'PayPal Inc.',
        icon: 'paypal',
      ),
      PaymentMethod(
        id: 'pay_3',
        name: 'Bank Transfer',
        provider: 'Przelewy24',
        icon: 'account_balance',
      ),
    ];
  }
}

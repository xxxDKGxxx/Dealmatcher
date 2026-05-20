import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/api/api_cart.dart';
import 'package:frontend/models/cart_item.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/delivery_method.dart';
import 'package:frontend/models/payment_method.dart';
import 'package:frontend/models/price.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/pages/order_summary_page.dart';

class FakeApiCart extends ApiCart {
  final List<CartItem> items;
  final Price total;

  FakeApiCart({required this.items, required this.total});

  @override
  Future<List<CartItem>> getCart() async {
    return items;
  }

  @override
  Future<Price> getCartTotal() async {
    return total;
  }
}

void main() {
  final testDelivery = DeliveryMethod(
    name: 'Courier Express',
    price: 15.0,
    description: 'Next day delivery',
    estimatedDays: 1,
    id: '123',
  );

  final testPayment = PaymentMethod(
    name: 'Credit Card',
    provider: 'Stripe',
    id: '1234',
    icon: '',
  );

  final testPrice = Price(value: 115.0, currency: 'PLN');

  final testItems = [
    CartItem(
      quantity: 2,
      offer: Offer(
        id: 0,
        title: 'Test Product',
        description: 'Description for offer',
        price: 100.0,
        images: [],
        seller: const Seller(id: 1, name: 'Test Seller'),
        category: Category(id: 1, name: 'Test Category', description: ''),
        tags: ['test'],
        properties: {},
        availability: 1,
        status: OfferStatus.active,
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
      ),
      id: 1,
      addedAt: DateTime.now(),
    ),
  ];

  group('OrderSummaryPage Tests', () {
    testWidgets('Should display loading indicator initially', (
      WidgetTester tester,
    ) async {
      final fakeApi = FakeApiCart(items: testItems, total: testPrice);

      await tester.pumpWidget(
        MaterialApp(
          home: OrderSummaryPage(
            apiCart: fakeApi,
            deliveryMethod: testDelivery,
            paymentMethod: testPayment,
          ),
        ),
      );

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('Should display order details after loading', (
      WidgetTester tester,
    ) async {
      final fakeApi = FakeApiCart(items: testItems, total: testPrice);

      await tester.pumpWidget(
        MaterialApp(
          home: OrderSummaryPage(
            apiCart: fakeApi,
            deliveryMethod: testDelivery,
            paymentMethod: testPayment,
          ),
        ),
      );

      await tester.pumpAndSettle();

      expect(find.text('Order Summary'), findsOneWidget);
      expect(find.text('Test Product'), findsOneWidget);
      expect(find.text('Quantity: 2'), findsOneWidget);

      expect(find.text('Courier Express'), findsOneWidget);
      expect(find.text('\$15.00'), findsOneWidget);

      expect(find.text('115.0 PLN'), findsOneWidget);
      expect(find.text('Place Order'), findsOneWidget);
    });

    testWidgets('Should show empty cart message when no items', (
      WidgetTester tester,
    ) async {
      final fakeApi = FakeApiCart(
        items: [],
        total: Price(value: 0, currency: 'PLN'),
      );

      await tester.pumpWidget(
        MaterialApp(
          home: OrderSummaryPage(
            apiCart: fakeApi,
            deliveryMethod: testDelivery,
            paymentMethod: testPayment,
          ),
        ),
      );

      await tester.pumpAndSettle();

      expect(find.text('Your cart is empty.'), findsOneWidget);
    });
  });
}

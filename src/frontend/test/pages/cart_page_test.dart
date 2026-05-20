import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/api/api_cart.dart';
import 'package:frontend/models/cart_item.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/price.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/pages/cart_page.dart';

class MockApiCart implements ApiCart {
  List<CartItem> mockItems = [];
  Price mockPrice = Price(value: 0.0, currency: 'PLN');

  bool shouldThrowError = false;
  bool updateShouldThrowError = false;
  String errorMessage = 'Exception: Something went wrong';

  int getCartCalls = 0;
  int getCartTotalCalls = 0;

  @override
  Future<List<CartItem>> getCart() async {
    getCartCalls++;
    if (shouldThrowError) throw Exception(errorMessage);
    return mockItems;
  }

  @override
  Future<Price> getCartTotal() async {
    getCartTotalCalls++;
    if (shouldThrowError) throw Exception(errorMessage);
    return mockPrice;
  }

  @override
  Future<CartItem> updateItemQuantity(int id, int quantity) async {
    if (updateShouldThrowError) throw Exception(errorMessage);
    if (shouldThrowError) throw Exception(errorMessage);

    final index = mockItems.indexWhere((item) => item.id == id);
    if (index != -1) {
      mockItems[index] = CartItem(
        id: mockItems[index].id,
        quantity: quantity,
        offer: mockItems[index].offer,
        addedAt: mockItems[index].addedAt,
      );
      return mockItems[index];
    }
    throw Exception('Invalid item');
  }

  @override
  Future<void> removeItem(int id) async {
    if (shouldThrowError) throw Exception(errorMessage);
    mockItems.removeWhere((item) => item.id == id);
  }

  @override
  Future<CartItem> addToCart(int offerId, {int quantity = 1}) {
    throw UnimplementedError();
  }
}

void main() {
  late MockApiCart mockApiCart;

  setUp(() {
    mockApiCart = MockApiCart();
    mockApiCart.mockPrice = Price(value: 150.0, currency: 'PLN');
    mockApiCart.mockItems = [
      CartItem(
        id: 1,
        quantity: 2,
        offer: Offer(
          id: 0,
          title: 'Test Offer',
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
        addedAt: DateTime.now(),
      ),
    ];
  });

  Widget createWidgetUnderTest() {
    return MaterialApp(home: CartPage(apiCart: mockApiCart));
  }

  testWidgets('Displays loader and then cart data', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createWidgetUnderTest());
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    await tester.pumpAndSettle();

    expect(find.text('Cart'), findsOneWidget);
    expect(find.text('Test Offer'), findsOneWidget);
    expect(find.text('2'), findsOneWidget);
    expect(find.text('150.0 PLN'), findsOneWidget);
  });

  testWidgets('Displays empty cart message', (WidgetTester tester) async {
    mockApiCart.mockItems = [];
    mockApiCart.mockPrice = Price(value: 0.0, currency: 'PLN');

    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    expect(find.text('Your cart is empty.'), findsOneWidget);
  });

  testWidgets('Increasing item quantity refreshes cart data', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final addButton = find.byIcon(Icons.add_circle_outline);
    await tester.tap(addButton);
    await tester.pumpAndSettle();

    expect(mockApiCart.getCartCalls, equals(2));
  });

  testWidgets('Removing an item from cart', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final deleteButton = find.byIcon(Icons.delete_outline);
    await tester.tap(deleteButton);
    await tester.pumpAndSettle();

    expect(find.text('Your cart is empty.'), findsOneWidget);
  });

  testWidgets(
    'Displays error in SnackBar when API throws an exception on action',
    (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      mockApiCart.updateShouldThrowError = true;
      mockApiCart.errorMessage = 'Exception: Connection error';

      final addButton = find.byIcon(Icons.add_circle_outline);
      await tester.tap(addButton);

      await tester.pump();

      final snackBarFinder = find.byType(SnackBar);
      expect(snackBarFinder, findsOneWidget);

      final textWidget = find
          .descendant(of: snackBarFinder, matching: find.byType(Text))
          .first;
      final Text text = tester.widget<Text>(textWidget);

      expect(text.data, contains('Connection error'));
      expect(text.data, contains('Could not update quantity'));

      await tester.pumpAndSettle();
    },
  );
}

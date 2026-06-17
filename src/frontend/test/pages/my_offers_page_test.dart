import 'dart:async';
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/pages/my_offers_page.dart';

void main() {
  setUpAll(() {
    HttpOverrides.global = _MockHttpOverrides();
  });

  List<Offer> generateMockOffers(int count) {
    return List.generate(
      count,
      (index) => Offer(
        id: index,
        title: 'Test Offer $index',
        description: 'Description for offer $index',
        price: 100.0 + index,
        images: ['https://example.com/mock_image_$index.jpg'],
        seller: const Seller(id: 1, name: 'Test Seller'),
        category: Category(id: 1, name: 'Test Category', description: ''),
        tags: ['test'],
        properties: {},
        availability: 1,
        status: OfferStatus.active,
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
      ),
    );
  }

  Widget createWidgetUnderTest({Future<List<Offer>>? future}) {
    return MaterialApp(home: MyOffersPage(offersFuture: future));
  }

  group('MyOffersPage Widget Tests', () {
    testWidgets('Shows CircularProgressIndicator while loading', (
      tester,
    ) async {
      final completer = Completer<List<Offer>>();

      await tester.pumpWidget(createWidgetUnderTest(future: completer.future));

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('Displays header "My Offers" after loading data', (
      tester,
    ) async {
      final successFuture = Future.value(generateMockOffers(0));

      await tester.pumpWidget(createWidgetUnderTest(future: successFuture));
      await tester.pumpAndSettle();

      expect(find.text('My Offers'), findsOneWidget);
    });

    testWidgets('Correctly renders offer list', (tester) async {
      final mockOffers = generateMockOffers(3);
      final successFuture = Future.value(mockOffers);

      await tester.pumpWidget(createWidgetUnderTest(future: successFuture));
      await tester.pumpAndSettle();

      expect(find.byType(Card), findsNWidgets(3));

      expect(find.text('Test Offer 0'), findsOneWidget);
      expect(find.text('Test Offer 1'), findsOneWidget);
      expect(find.text('Test Offer 2'), findsOneWidget);
    });

    testWidgets('Correctly formats price and shows category', (tester) async {
      final mockOffers = [
        Offer(
          id: 1,
          title: 'Suer Laptop',
          description: 'Omega Ultra Fabulous Gaming Laptop',
          price: 4500.5,
          images: ['https://example.com/image.jpg'],
          seller: const Seller(id: 1, name: 'Seller'),
          category: Category(id: 1, name: 'Electronics', description: ''),
          tags: [],
          properties: {},
          availability: 1,
          status: OfferStatus.active,
          createdAt: DateTime.now(),
          updatedAt: DateTime.now(),
        ),
      ];

      await tester.pumpWidget(
        createWidgetUnderTest(future: Future.value(mockOffers)),
      );
      await tester.pumpAndSettle();

      expect(find.text('Category: Electronics'), findsOneWidget);
      expect(find.text('Price: 4500.50'), findsOneWidget);
    });

    testWidgets('Shows eye icon for sold offer and arrow for active offer', (
      tester,
    ) async {
      final mockOffers = [
        Offer(
          id: 1,
          title: 'Sold Laptop',
          description: 'Omega Ultra Fabulous Gaming Laptop',
          price: 4500.5,
          images: ['https://example.com/image.jpg'],
          seller: const Seller(id: 1, name: 'Seller'),
          category: Category(id: 1, name: 'Electronics', description: ''),
          tags: [],
          properties: {},
          availability: 1,
          status: OfferStatus.sold,
          createdAt: DateTime.now(),
          updatedAt: DateTime.now(),
        ),
        Offer(
          id: 2,
          title: 'Active Laptop',
          description: 'Omega Ultra Fabulous Gaming Laptop',
          price: 4500.5,
          images: ['https://example.com/image.jpg'],
          seller: const Seller(id: 1, name: 'Seller'),
          category: Category(id: 1, name: 'Electronics', description: ''),
          tags: [],
          properties: {},
          availability: 1,
          status: OfferStatus.active,
          createdAt: DateTime.now(),
          updatedAt: DateTime.now(),
        ),
      ];

      await tester.pumpWidget(
        createWidgetUnderTest(future: Future.value(mockOffers)),
      );
      await tester.pumpAndSettle();

      expect(find.byIcon(Icons.visibility), findsOneWidget);
      expect(find.byIcon(Icons.arrow_right_alt), findsOneWidget);
    });

    testWidgets('CustomScrollView scrolling shows hidden elements', (
      tester,
    ) async {
      final mockOffers = generateMockOffers(20);

      await tester.pumpWidget(
        createWidgetUnderTest(future: Future.value(mockOffers)),
      );
      await tester.pumpAndSettle();

      expect(find.text('Test Offer 0'), findsOneWidget);
      expect(find.text('Test Offer 19'), findsNothing);

      final scrollable = find.byType(Scrollable);

      await tester.drag(scrollable, const Offset(0, -2500));
      await tester.pumpAndSettle();

      expect(find.text('Test Offer 19'), findsOneWidget);
    });
  });
}

// image network mock
class _MockHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return _MockHttpClient();
  }
}

class _MockHttpClient implements HttpClient {
  @override
  bool autoUncompress = false;

  @override
  Future<HttpClientRequest> getUrl(Uri url) async {
    return _MockHttpClientRequest();
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _MockHttpClientRequest implements HttpClientRequest {
  @override
  Future<HttpClientResponse> close() async {
    return _MockHttpClientResponse();
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

class _MockHttpClientResponse implements HttpClientResponse {
  @override
  int get statusCode => 200;

  @override
  int get contentLength => _transparentImage.length;

  @override
  HttpClientResponseCompressionState get compressionState =>
      HttpClientResponseCompressionState.notCompressed;

  @override
  StreamSubscription<List<int>> listen(
    void Function(List<int> event)? onData, {
    Function? onError,
    void Function()? onDone,
    bool? cancelOnError,
  }) {
    return Stream<List<int>>.fromIterable([_transparentImage]).listen(
      onData,
      onError: onError,
      onDone: onDone,
      cancelOnError: cancelOnError,
    );
  }

  @override
  dynamic noSuchMethod(Invocation invocation) => super.noSuchMethod(invocation);
}

// Byte encoded, correct, transparent PNG 1x1 picture
final List<int> _transparentImage = <int>[
  0x89,
  0x50,
  0x4E,
  0x47,
  0x0D,
  0x0A,
  0x1A,
  0x0A,
  0x00,
  0x00,
  0x00,
  0x0D,
  0x49,
  0x48,
  0x44,
  0x52,
  0x00,
  0x00,
  0x00,
  0x01,
  0x00,
  0x00,
  0x00,
  0x01,
  0x08,
  0x06,
  0x00,
  0x00,
  0x00,
  0x1F,
  0x15,
  0xC4,
  0x89,
  0x00,
  0x00,
  0x00,
  0x0A,
  0x49,
  0x44,
  0x41,
  0x54,
  0x78,
  0x9C,
  0x63,
  0x00,
  0x01,
  0x00,
  0x00,
  0x05,
  0x00,
  0x01,
  0x0D,
  0x0A,
  0x2D,
  0xB4,
  0x00,
  0x00,
  0x00,
  0x00,
  0x49,
  0x45,
  0x4E,
  0x44,
  0xAE,
  0x42,
  0x60,
  0x82,
];

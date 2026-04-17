import 'dart:async';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/pages/offer_details_page.dart';

// Helper mock to avoid NetworkImage exceptions during widget tests
class _MockHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return _MockHttpClient();
  }
}

class _MockHttpClient implements HttpClient {
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

const List<int> _transparentImage = [
  0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49,
  0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x06,
  0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4, 0x89, 0x00, 0x00, 0x00, 0x0A, 0x49, 0x44,
  0x41, 0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00, 0x05, 0x00, 0x01, 0x0D,
  0x0A, 0x2D, 0xB4, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42,
  0x60, 0x82,
];

void main() {
  setUpAll(() {
    HttpOverrides.global = _MockHttpOverrides();
  });

  Widget createWidgetUnderTest() {
    return const MaterialApp(
      home: OfferDetailsPage(offerId: 1),
    );
  }

  testWidgets('shows loading indicator initially', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());

    // Initially, Future is not resolved, so it should show a loader
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    expect(find.text('High Performance Gaming Laptop'), findsNothing);

    // Clear pending timers
    await tester.pumpAndSettle(const Duration(seconds: 2));
  });

  testWidgets('displays offer details after data is loaded', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());

    // Wait for the mock delays to complete (1 second for offer + 0.5s for properties)
    await tester.pumpAndSettle(const Duration(seconds: 2));

    // Loading indicator should be gone
    expect(find.byType(CircularProgressIndicator), findsNothing);

    // Verify basic info
    expect(find.text('High Performance Gaming Laptop'), findsOneWidget);
    expect(find.text('4500.00 PLN'), findsOneWidget);
    expect(find.text('ACTIVE'), findsOneWidget); // Status
    expect(find.text('Laptops'), findsOneWidget); // Category
    expect(find.text('A powerful gaming laptop with the latest components, perfect for gaming and professional workloads. Lightly used, excellent condition.'), findsOneWidget);

    // Verify properties
    expect(find.text('Intel Core i9-13900HX'), findsOneWidget);
    expect(find.text('32'), findsOneWidget);
    expect(find.text('1000'), findsOneWidget);
    expect(find.text('Windows 11 Home'), findsOneWidget);

    // Verify seller
    expect(find.text('TechStore Poland'), findsOneWidget);

    // Verify tags
    expect(find.text('#Gaming'), findsOneWidget);
    expect(find.text('#RTX'), findsOneWidget);

    // Verify action buttons
    expect(find.text('ADD TO CART'), findsOneWidget);
    expect(find.text('CONTACT SELLER'), findsOneWidget);
  });

  testWidgets('verifies boolean property icon is displayed', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));

    // Boolean property "Is New" has value "false", so it should display a cancel icon
    expect(find.byIcon(Icons.cancel), findsOneWidget);
    // Since "false" is mapped to Icons.cancel (red), we can just find the icon
  });
}

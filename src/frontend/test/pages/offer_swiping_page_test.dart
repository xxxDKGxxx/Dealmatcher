import 'dart:async';
import 'dart:io';
import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/pages/offers_swiping_page.dart';
import 'package:frontend/widgets/offer_filter_widget.dart';

void main() {
  setUpAll(() {
    HttpOverrides.global = _MockHttpOverrides();
  });

  Widget createTestWidget() {
    return MaterialApp(
      theme: ThemeData.dark(),
      home: const OffersSwipingPage(),
    );
  }

  testWidgets('Should display loader and then show first offer', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createTestWidget());

    expect(find.byType(CircularProgressIndicator), findsOneWidget);

    await tester.pump(const Duration(milliseconds: 1000));
    await tester.pumpAndSettle();

    expect(find.text('Apple Mac Pro'), findsOneWidget);
    expect(find.text('262000.00 zł'), findsOneWidget);
  });

  testWidgets('Should swipe to the next offer', (WidgetTester tester) async {
    await tester.pumpWidget(createTestWidget());

    await tester.pump(const Duration(milliseconds: 1000));
    await tester.pumpAndSettle();

    expect(find.text('Apple Mac Pro'), findsOneWidget);

    await tester.drag(find.byType(PageView), const Offset(0, -800));
    await tester.pumpAndSettle();

    expect(find.text('ThinkPad T500'), findsOneWidget);
    expect(find.text('420.00 zł'), findsOneWidget);
  });

  testWidgets('Should open filter bottom sheet', (WidgetTester tester) async {
    await tester.pumpWidget(createTestWidget());
    await tester.pump(const Duration(milliseconds: 1000));
    await tester.pumpAndSettle();

    final filterButton = find.byIcon(Icons.filter_list);
    await tester.tap(filterButton);
    await tester.pumpAndSettle();

    expect(find.byType(OfferFilterWidget), findsOneWidget);
  });

  testWidgets('Should show empty state message', (WidgetTester tester) async {
    await tester.pumpWidget(createTestWidget());
    await tester.pump(const Duration(milliseconds: 1000));
    await tester.pumpAndSettle();

    await tester.tap(find.byIcon(Icons.filter_list));
    await tester.pumpAndSettle();

    await tester.enterText(find.byType(TextField).first, 'NonExistentProduct');

    await tester.pump(const Duration(milliseconds: 1000));
    await tester.pumpAndSettle();

    await tester.tapAt(const Offset(10, 10));
    await tester.pumpAndSettle();

    expect(find.text('No offers matching the criteria.'), findsOneWidget);
  });
}

class _MockHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return _MockHttpClient();
  }
}

class _MockHttpClient extends Fake implements HttpClient {
  @override
  Future<HttpClientRequest> getUrl(Uri url) async => _MockHttpClientRequest();
}

class _MockHttpClientRequest extends Fake implements HttpClientRequest {
  @override
  Future<HttpClientResponse> close() async => _MockHttpClientResponse();
}

class _MockHttpClientResponse extends Fake implements HttpClientResponse {
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
}

final Uint8List _transparentImage = Uint8List.fromList([
  0x47,
  0x49,
  0x46,
  0x38,
  0x39,
  0x61,
  0x01,
  0x00,
  0x01,
  0x00,
  0x80,
  0x00,
  0x00,
  0xFF,
  0xFF,
  0xFF,
  0x00,
  0x00,
  0x00,
  0x21,
  0xf9,
  0x04,
  0x01,
  0x00,
  0x00,
  0x00,
  0x00,
  0x2c,
  0x00,
  0x00,
  0x00,
  0x00,
  0x01,
  0x00,
  0x01,
  0x00,
  0x00,
  0x02,
  0x02,
  0x44,
  0x01,
  0x00,
  0x3b,
]);

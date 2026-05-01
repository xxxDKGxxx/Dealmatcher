import 'dart:async';
import 'dart:io';
import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/pages/offers_swiping_page.dart';
import 'package:frontend/widgets/offer_filter_widget.dart';
import 'package:frontend/api/api_core.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  setUpAll(() {
    HttpOverrides.global = _MockHttpOverrides();
    SharedPreferences.setMockInitialValues({});
    ApiCore().init('http://localhost:8080');
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

    await tester.drag(find.byType(Dismissible), const Offset(-800, 0));
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

    expect(
      find.text("No offers matching the criteria. You've seen everything!"),
      findsOneWidget,
    );
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
  Future<HttpClientRequest> getUrl(Uri url) async =>
      _MockHttpClientRequest(url);

  @override
  Future<HttpClientRequest> postUrl(Uri url) async =>
      _MockHttpClientRequest(url);

  @override
  Future<HttpClientRequest> openUrl(String method, Uri url) async =>
      _MockHttpClientRequest(url);

  @override
  void close({bool force = false}) {}

  @override
  dynamic noSuchMethod(Invocation invocation) {
    return null;
  }
}

class _MockHttpClientRequest extends Fake implements HttpClientRequest {
  _MockHttpClientRequest(this.url);
  final Uri url;

  bool _followRedirects = true;
  int _maxRedirects = 5;
  int _contentLength = -1;

  @override
  final HttpHeaders headers = _MockHttpHeaders();

  @override
  bool get followRedirects => _followRedirects;

  @override
  set followRedirects(bool value) => _followRedirects = value;

  @override
  int get maxRedirects => _maxRedirects;

  @override
  set maxRedirects(int value) => _maxRedirects = value;

  @override
  int get contentLength => _contentLength;

  @override
  set contentLength(int value) => _contentLength = value;

  bool _persistentConnection = true;

  @override
  bool get persistentConnection => _persistentConnection;

  @override
  set persistentConnection(bool value) => _persistentConnection = value;

  @override
  void add(List<int> data) {
    _bodyData.addAll(data);
  }

  @override
  Future<void> addStream(Stream<List<int>> stream) async {
    await for (final chunk in stream) {
      _bodyData.addAll(chunk);
    }
  }

  final List<int> _bodyData = [];

  @override
  void write(Object? obj) {
    if (obj != null) {
      _bodyData.addAll(obj.toString().codeUnits);
    }
  }

  @override
  Future<HttpClientResponse> get done => close();

  @override
  Future<HttpClientResponse> close() async {
    if (url.path.contains('/api/v1/offers/search')) {
      final bodyStr = String.fromCharCodes(_bodyData);
      if (bodyStr.contains('NonExistentProduct')) {
        return _MockHttpClientResponse.emptyJson();
      }
      return _MockHttpClientResponse.json();
    }
    if (url.path.contains('/api/v1/categories')) {
      return _MockHttpClientResponse.categoriesJson();
    }
    return _MockHttpClientResponse.image();
  }

  @override
  dynamic noSuchMethod(Invocation invocation) {
    return null;
  }
}

class _MockHttpHeaders extends Fake implements HttpHeaders {
  int _contentLength = -1;
  bool _chunkedTransferEncoding = false;

  @override
  void set(String name, Object value, {bool preserveHeaderCase = false}) {}

  @override
  void add(String name, Object value, {bool preserveHeaderCase = false}) {}

  @override
  int get contentLength => _contentLength;

  @override
  set contentLength(int value) => _contentLength = value;

  @override
  bool get chunkedTransferEncoding => _chunkedTransferEncoding;

  @override
  set chunkedTransferEncoding(bool value) => _chunkedTransferEncoding = value;

  @override
  void forEach(void Function(String name, List<String> values) action) {
    action('content-type', ['application/json; charset=utf-8']);
  }

  @override
  dynamic noSuchMethod(Invocation invocation) {
    return null;
  }
}

class _MockHttpClientResponse extends Fake implements HttpClientResponse {
  _MockHttpClientResponse(this._data);

  @override
  final HttpHeaders headers = _MockHttpHeaders();

  @override
  bool get isRedirect => false;

  @override
  List<RedirectInfo> get redirects => [];

  @override
  bool get persistentConnection => true;

  @override
  String get reasonPhrase => 'OK';

  factory _MockHttpClientResponse.image() {
    return _MockHttpClientResponse(_transparentImage);
  }

  factory _MockHttpClientResponse.json() {
    final jsonStr = '''
    [
        {
          "id": 1,
          "title": "Apple Mac Pro",
          "description": "Mac Pro with M2 Ultra",
          "price": 262000.0,
          "images": ["https://example.com/macpro.jpg"],
          "seller": {"id": 1, "name": "Tim Cook"},
          "category": {"id": 1, "name": "Computers", "description": ""},
          "tags": ["apple", "mac"],
          "properties": {},
          "availability": 10,
          "status": "active",
          "createdAt": "2023-01-01T00:00:00Z",
          "updatedAt": "2023-01-01T00:00:00Z"
        },
        {
          "id": 2,
          "title": "ThinkPad T500",
          "description": "Classic IBM ThinkPad",
          "price": 420.0,
          "images": ["https://example.com/thinkpad.jpg"],
          "seller": {"id": 2, "name": "IBM Fan"},
          "category": {"id": 1, "name": "Computers", "description": ""},
          "tags": ["lenovo", "thinkpad"],
          "properties": {},
          "availability": 1,
          "status": "active",
          "createdAt": "2023-01-01T00:00:00Z",
          "updatedAt": "2023-01-01T00:00:00Z"
        }
    ]
    ''';
    return _MockHttpClientResponse(Uint8List.fromList(jsonStr.codeUnits));
  }

  factory _MockHttpClientResponse.emptyJson() {
    return _MockHttpClientResponse(Uint8List.fromList('[]'.codeUnits));
  }

  factory _MockHttpClientResponse.categoriesJson() {
    final jsonStr = '''
    [
      {"id": 1, "name": "Computers", "description": ""}
    ]
    ''';
    return _MockHttpClientResponse(Uint8List.fromList(jsonStr.codeUnits));
  }

  final Uint8List _data;

  @override
  int get statusCode => 200;

  @override
  int get contentLength => _data.length;

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
    return Stream<List<int>>.fromIterable([_data]).listen(
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

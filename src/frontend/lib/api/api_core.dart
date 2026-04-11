import 'package:frontend/api/api_urls.dart';
import 'package:frontend/api/models/request_model.dart';
import 'package:http/http.dart' as http;

class ApiCore {
  static final ApiCore _instance = ApiCore._internal();
  factory ApiCore() => _instance;
  ApiCore._internal();

  final String _apiUrl = ApiUrls().apiUrl;

  String? _baseUrl;
  String? _token;

  void init(String baseUrl) {
    _baseUrl = baseUrl;
  }

  void setToken(String? token) {
    _token = token;
  }

  void nullToken() {
    _token = null;
  }

  bool get isAuthenticated => _token != null;

  Map<String, String> get _headers {
    final headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
    if (_token != null) {
      headers['Authorization'] = 'Bearer $_token';
    }
    return headers;
  }

  Uri _getUri(String endpoint) {
    if (_baseUrl == null) {
      throw Exception("ApiCore has not been initialized.");
    }
    return Uri.parse('$_baseUrl$_apiUrl$endpoint');
  }

  // intercept response to check status code
  Future<http.Response> intercept(
    Future<http.Response> Function() httpMethod,
  ) async {
    final response = await httpMethod();
    switch (response.statusCode) {
      case 401:
        {
          // TODO: add running check and logout if 401 received
          // now it logs out only when page is changed but should every time a 401 is received
          nullToken();
        }
    }
    return response;
  }

  // HTTP methods
  Future<http.Response> get(String endpoint) async {
    return await intercept(
      () async => http.get(_getUri(endpoint), headers: _headers),
    );
  }

  Future<http.Response> post(String endpoint, RequestModel request) async {
    return await intercept(
      () async => http.post(
        _getUri(endpoint),
        headers: _headers,
        body: request.toJson(),
      ),
    );
  }

  Future<http.Response> put(String endpoint, RequestModel request) async {
    return await intercept(
      () async => http.put(
        _getUri(endpoint),
        headers: _headers,
        body: request.toJson(),
      ),
    );
  }

  Future<http.Response> patch(String endpoint, RequestModel request) async {
    return await intercept(
      () async => http.patch(
        _getUri(endpoint),
        headers: _headers,
        body: request.toJson(),
      ),
    );
  }

  Future<http.Response> delete(String endpoint) async {
    return await intercept(
      () => http.delete(_getUri(endpoint), headers: _headers),
    );
  }
}

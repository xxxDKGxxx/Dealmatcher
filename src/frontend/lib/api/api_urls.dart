class ApiUrls {
  String get apiUrl => '/api/v1';
  String get login => '/users/login';
  String get register => '/users/register';
  String get profile => '/users/me';
  String get categories => '/categories';

  String offerDetails(int offerId) => '/offers/$offerId';
  String properties(String categoryName) => '/categories/$categoryName/properties';
}

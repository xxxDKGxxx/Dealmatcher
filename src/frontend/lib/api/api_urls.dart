class ApiUrls {
  String get apiUrl => '/api/v1';
  String get login => '/users/login';
  String get register => '/users/register';
  String get profile => '/users/me';
  String get myOffers => '/users/me/offers';
  String get offers => '/offers';
  String get categories => '/categories';

  String offerDetailsById(int id) => '/offers/$id';
  String propertiesByCategoryName(String categoryName) =>
      '/categories/$categoryName/properties';
  String offerById(int offerId) => '/offers/$offerId';
}

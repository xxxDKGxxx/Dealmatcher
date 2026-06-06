class ApiUrls {
  String get apiUrl => '/api/v1';
  String get login => '/users/login';
  String get register => '/users/register';
  String get profile => '/users/me';
  String get myOffers => '/users/me/offers';
  String get offers => '/offers';
  String get searchOffers => '/offers/search';
  String get categories => '/categories';
  String get conversations => '/conversations';
  String get cartItems => '/cart/items';
  String get cartTotal => '/cart/total';
  String get adminGetOffers => '/admin/offers';
  String get adminGetUsers => '/admin/users';
  String get deliveryMethods => '/purchases/delivery-methods';
  String get paymentMethods => '/purchases/payment-methods';

  String offerDetailsById(int id) => '/offers/$id';
  String conversationById(int id) => '/conversations/$id';
  String messagesByConversationId(int id) => '/conversations/$id/messages';
  String propertiesByCategoryName(String categoryName) =>
      '/categories/$categoryName/properties';
  String offerById(int offerId) => '/offers/$offerId';
  String cartItemById(int cartItemId) => '/cart/items/$cartItemId';
  String adminUserActivity(int userId) => '/admin/activity/user/$userId';
}

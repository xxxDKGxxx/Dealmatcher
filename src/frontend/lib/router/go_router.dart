import 'package:frontend/api/api_core.dart';
import 'package:frontend/models/delivery_method.dart';
import 'package:frontend/models/payment_method.dart';
import 'package:frontend/pages/admin_view_page.dart';
import 'package:frontend/pages/cart_page.dart';
import 'package:frontend/pages/conversation_page.dart';
import 'package:frontend/pages/conversations_list_page.dart';
import 'package:frontend/pages/create_update_offer_page.dart';
import 'package:frontend/pages/home_page.dart';
import 'package:frontend/pages/login_page.dart';
import 'package:frontend/pages/my_offers_page.dart';
import 'package:frontend/pages/offer_details_page.dart';
import 'package:frontend/pages/order_summary_page.dart';
import 'package:frontend/pages/profile_edit_page.dart';
import 'package:frontend/pages/profile_page.dart';
import 'package:frontend/pages/register_page.dart';
import 'package:go_router/go_router.dart';

final GoRouter globalRouter = GoRouter(
  initialLocation: '/',
  redirect: (context, state) {
    final notAuthenticated = !ApiCore().isAuthenticated;
    final currentPage = state.fullPath!.split('/').last;
    const allowedUnauthenticatedPages = ['login', 'register'];
    final notOnAllowedPage = !allowedUnauthenticatedPages.contains(currentPage);

    if (notAuthenticated && notOnAllowedPage) {
      ApiCore().nullToken();
      return '/login';
    }
    return null;
  },
  routes: [
    GoRoute(path: '/add-offer', builder: (context, state) => CreateOfferPage()),
    GoRoute(path: '/', builder: (context, state) => HomePage()),
    GoRoute(path: '/register', builder: (context, state) => RegisterPage()),
    GoRoute(path: '/login', builder: (context, state) => LoginPage()),
    GoRoute(path: '/profile', builder: (context, state) => ProfilePage()),
    GoRoute(path: '/cart', builder: (context, state) => CartPage()),
    GoRoute(path: '/admin', builder: (context, state) => AdminViewPage()),
    GoRoute(
      path: '/profile-edit',
      builder: (context, state) => ProfileEditPage(),
    ),
    GoRoute(path: '/my-offers', builder: (context, state) => MyOffersPage()),
    GoRoute(
      path: '/offer/:id',
      builder: (context, state) {
        final id = int.parse(state.pathParameters['id']!);
        return OfferDetailsPage(offerId: id);
      },
    ),
    GoRoute(
      path: '/conversations',
      builder: (context, state) => const ConversationsListPage(),
    ),
    GoRoute(
      path: '/conversation/:id',
      builder: (context, state) {
        final id = int.tryParse(state.pathParameters['id'] ?? '');
        return ConversationPage(conversationId: id!);
      },
    ),
    GoRoute(
      path: '/update-offer/:id',
      builder: (context, state) {
        final id = int.parse(state.pathParameters['id']!);
        return CreateOfferPage(offerId: id);
      },
    ),
    GoRoute(
      path: '/order-summary',
      builder: (context, state) {
        final extras = state.extra as Map<String, dynamic>;

        return OrderSummaryPage(
          deliveryMethod: extras['deliveryMethod'] as DeliveryMethod,
          paymentMethod: extras['paymentMethod'] as PaymentMethod,
        );
      },
    ),
  ],
);

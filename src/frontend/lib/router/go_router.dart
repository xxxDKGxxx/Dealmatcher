import 'package:frontend/api/api_core.dart';
import 'package:frontend/pages/create_offer_page.dart';
import 'package:frontend/pages/home_page.dart';
import 'package:frontend/pages/login_page.dart';
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
  ],
);

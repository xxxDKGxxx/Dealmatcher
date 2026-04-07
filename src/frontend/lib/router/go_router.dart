import 'package:frontend/pages/offer/create_offer_page.dart';
import 'package:frontend/pages/home_page.dart';
import 'package:frontend/pages/auth/login_page.dart';
import 'package:frontend/pages/user/register_page.dart';
import 'package:go_router/go_router.dart';

final GoRouter globalRouter = GoRouter(
  initialLocation: '/login',
  routes: [
    GoRoute(path: '/add-offer', builder: (context, state) => CreateOfferPage()),
    GoRoute(path: '/', builder: (context, state) => HomePage()),
    GoRoute(path: '/register', builder: (context, state) => RegisterPage()),
    GoRoute(path: '/login', builder: (context, state) => LoginPage()),
  ],
);

import 'package:frontend/pages/home_page.dart';
import 'package:frontend/pages/register_page.dart';
import 'package:go_router/go_router.dart';

final GoRouter globalRouter = GoRouter(
  initialLocation: '/register',
  routes: [
    GoRoute(path: '/', builder: (context, state) => HomePage()),
    GoRoute(path: '/register', builder: (context, state) => RegisterPage()),
  ],
);

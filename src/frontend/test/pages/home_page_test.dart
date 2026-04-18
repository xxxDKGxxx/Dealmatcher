import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/pages/home_page.dart';

void main() {
  void setDesktopSize(WidgetTester tester) {
    tester.view.physicalSize = const Size(1200, 1200);
    tester.view.devicePixelRatio = 1.0;
  }

  Widget createWidgetUnderTest() {
    final router = GoRouter(
      initialLocation: '/',
      routes: [
        GoRoute(
          path: '/',
          builder: (context, state) => const HomePage(),
        ),
        GoRoute(
          path: '/add-offer',
          builder: (context, state) => const Scaffold(body: Text('Add Offer Page')),
        ),
        GoRoute(
          path: '/profile',
          builder: (context, state) => const Scaffold(body: Text('Profile Page')),
        ),
        GoRoute(
          path: '/login',
          builder: (context, state) => const Scaffold(body: Text('Login Page')),
        ),
      ],
    );

    return MaterialApp.router(
      routerConfig: router,
    );
  }

  testWidgets('Welcome test and add offer icon on home page', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    expect(find.text('Welcome to Home Page'), findsOneWidget);
    expect(find.byIcon(Icons.add_circle_outline), findsOneWidget);
  });

  testWidgets('Add offer navigation', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final addButton = find.byIcon(Icons.add_circle_outline);
    await tester.tap(addButton);
    await tester.pumpAndSettle();

    expect(find.text('Add Offer Page'), findsOneWidget);
  });

  testWidgets('Drawer opening and navigation to profile page', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final drawerIcon = find.byIcon(Icons.menu);
    await tester.tap(drawerIcon);
    await tester.pumpAndSettle();

    expect(find.text('Menu'), findsOneWidget);

    final profileTile = find.text('Profile');
    await tester.tap(profileTile);
    await tester.pumpAndSettle();

    expect(find.text('Profile Page'), findsOneWidget);
  });

  testWidgets('Logout cleans session and shows snackbar', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();
    await tester.tap(find.byIcon(Icons.menu));
    await tester.pumpAndSettle();
    final logoutTile = find.text('Log out');
    await tester.tap(logoutTile);
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 500)); // Czas dla SnackBara

    expect(find.text('Login Page'), findsOneWidget);
    expect(find.text('Logged out'), findsOneWidget);
  });
}
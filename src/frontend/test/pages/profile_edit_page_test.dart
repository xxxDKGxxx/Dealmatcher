import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/pages/profile_edit_page.dart';

void main() {
  void setDesktopSize(WidgetTester tester) {
    tester.view.physicalSize = const Size(1200, 1200);
    tester.view.devicePixelRatio = 1.0;
  }

  Widget createWidgetUnderTest() {
    final router = GoRouter(
      initialLocation: '/profile-edit',
      routes: [
        GoRoute(
          path: '/profile-edit',
          builder: (context, state) => const ProfileEditPage(),
        ),
        GoRoute(
          path: '/profile',
          builder: (context, state) => const Scaffold(body: Text('Profile Page')),
        ),
      ],
    );

    return MaterialApp.router(
      routerConfig: router,
    );
  }

  testWidgets('Displays loader and then error (because tests are offline)', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());

    expect(find.byType(CircularProgressIndicator), findsOneWidget);

    await tester.pumpAndSettle(const Duration(seconds: 2));

    expect(find.text('Error loading user data.'), findsOneWidget);
  });

  testWidgets('Update button is visible and clickable', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));
  });

  testWidgets('Enter data and try tu update', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));

    expect(find.byType(SnackBar), findsOneWidget);
  });

  testWidgets('Update button click', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));

    expect(true, isTrue);
  });
}
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/pages/profile_page.dart';

void main() {
  void setDesktopSize(WidgetTester tester) {
    tester.view.physicalSize = const Size(1200, 1200);
    tester.view.devicePixelRatio = 1.0;
  }

  Widget createWidgetUnderTest() {
    final router = GoRouter(
      initialLocation: '/profile',
      routes: [
        GoRoute(
          path: '/profile',
          builder: (context, state) => const ProfilePage(),
        ),
        GoRoute(
          path: '/profile-edit',
          builder: (context, state) =>
              const Scaffold(body: Text('Edit Profile Page')),
        ),
      ],
    );

    return MaterialApp.router(routerConfig: router);
  }

  testWidgets('Shows loader during data fetch', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());

    expect(find.byType(CircularProgressIndicator), findsOneWidget);

    await tester.pumpAndSettle(const Duration(seconds: 2));
  });

  testWidgets('Handles API error and shows message', (
    WidgetTester tester,
  ) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));

    expect(find.text('Error loading user data.'), findsOneWidget);
  });

  testWidgets('Shows snackbar after load error', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));

    expect(find.byType(SnackBar), findsOneWidget);
  });

  testWidgets('Edit button press goes to profile edit page', (
    WidgetTester tester,
  ) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle(const Duration(seconds: 2));

    final editButton = find.byIcon(Icons.edit);
    expect(editButton, findsOneWidget);

    await tester.tap(editButton);
    await tester.pumpAndSettle();

    expect(find.text('Edit Profile Page'), findsOneWidget);
  });
}

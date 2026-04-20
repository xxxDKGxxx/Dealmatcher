import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/pages/login_page.dart';

void main() {
  Widget createWidgetUnderTest() {
    final router = GoRouter(
      initialLocation: '/login',
      routes: [
        GoRoute(path: '/login', builder: (context, state) => const LoginPage()),
        GoRoute(
          path: '/register',
          builder: (context, state) =>
              const Scaffold(body: Text('register page')),
        ),
      ],
    );

    return MaterialApp.router(routerConfig: router);
  }

  testWidgets('Display default UI elements', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    expect(find.text('Login'), findsWidgets);
    expect(find.text("Don't have account? Click here."), findsOneWidget);
    expect(find.byType(ElevatedButton), findsOneWidget);
  });

  testWidgets('Registration link click goes to registration page', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final registerLink = find.text("Don't have account? Click here.");
    expect(registerLink, findsOneWidget);

    await tester.tap(registerLink);
    await tester.pumpAndSettle();
    expect(find.text('register page'), findsOneWidget);
  });

  testWidgets('Filling form and clicking Login shows snackbar', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final textFields = find.byType(TextFormField);

    if (textFields.evaluate().length >= 2) {
      await tester.enterText(textFields.at(0), 'test@test.com');
      await tester.enterText(textFields.at(1), 'superhaslo123');
    }

    final loginButton = find.byType(ElevatedButton);
    await tester.tap(loginButton);
    await tester.pump();
    await tester.pump(const Duration(seconds: 1));
    expect(find.byType(SnackBar), findsOneWidget);
  });
}

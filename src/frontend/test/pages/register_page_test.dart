import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/pages/register_page.dart';

void main() {
  void setDesktopSize(WidgetTester tester) {
    tester.view.physicalSize = const Size(1200, 1200);
    tester.view.devicePixelRatio = 1.0;
  }

  tearDown(() {
  });

  Widget createWidgetUnderTest() {
    final router = GoRouter(
      initialLocation: '/register',
      routes: [
        GoRoute(
          path: '/register',
          builder: (context, state) => const RegisterPage(),
        ),
        GoRoute(
          path: '/',
          builder: (context, state) => const Scaffold(body: Text('home page')),
        ),
      ],
    );

    return MaterialApp.router(
      routerConfig: router,
    );
  }

  testWidgets('Display UI', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    expect(find.text('Register'), findsWidgets);
    expect(find.byType(ElevatedButton), findsOneWidget);
  });

  testWidgets('Empty form validation', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final registerButton = find.byType(ElevatedButton);

    await tester.ensureVisible(registerButton);
    await tester.tap(registerButton);
    await tester.pump();

    expect(find.byType(SnackBar), findsNothing);
  });

  testWidgets('Passwords throw error', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final textFields = find.byType(TextFormField);

    await tester.enterText(textFields.at(3), 'Passwd123');
    await tester.enterText(textFields.at(4), 'never gonna give you up');

    final registerButton = find.byType(ElevatedButton);
    await tester.ensureVisible(registerButton);
    await tester.tap(registerButton);
    await tester.pumpAndSettle();

    expect(find.text('Repeated password is different'), findsOneWidget);
  });

  testWidgets('Show error snackbar', (WidgetTester tester) async {
    setDesktopSize(tester);
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final textFields = find.byType(TextFormField);

    await tester.enterText(textFields.at(0), 'John');
    await tester.enterText(textFields.at(1), 'Pork');
    await tester.enterText(textFields.at(2), 'piekarna@pawelek.wolomin');
    await tester.enterText(textFields.at(3), 'favpasswd');
    await tester.enterText(textFields.at(4), 'favpasswd');

    final registerButton = find.byType(ElevatedButton);
    await tester.ensureVisible(registerButton);
    await tester.tap(registerButton);
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 500));

    expect(find.byType(SnackBar), findsOneWidget);
  });
}
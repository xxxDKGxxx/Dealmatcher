import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/widgets/admin_offer_activity_view.dart';

void main() {
  Widget createWidgetUnderTest() {
    return const MaterialApp(home: Scaffold(body: AdminOfferActivityView()));
  }

  testWidgets('Display default UI elements', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    // Check for "Offer ID" input
    expect(find.byType(TextFormField), findsOneWidget);
    expect(find.text('Offer ID'), findsOneWidget);

    // Check for "Search" button
    expect(find.widgetWithText(FilledButton, 'Search'), findsOneWidget);

    // Check for initial empty state text
    expect(find.text('Enter an Offer ID to view activities'), findsOneWidget);
  });

  testWidgets('Pressing Search with empty ID does nothing', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final searchButton = find.widgetWithText(FilledButton, 'Search');
    await tester.tap(searchButton);
    await tester.pumpAndSettle();

    // State shouldn't change, no error snackbar either since it just returns early
    expect(find.text('Enter an Offer ID to view activities'), findsOneWidget);
  });

  testWidgets('Entering non-numeric ID and searching shows snackbar', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final textInput = find.byType(TextFormField);
    await tester.enterText(textInput, 'invalid');
    await tester.pumpAndSettle();

    final searchButton = find.widgetWithText(FilledButton, 'Search');
    await tester.tap(searchButton);

    // Pump to show snackbar
    await tester.pump();
    await tester.pump(const Duration(seconds: 1));

    expect(find.byType(SnackBar), findsOneWidget);
    expect(find.text('Please enter a valid Offer ID'), findsOneWidget);
  });
}

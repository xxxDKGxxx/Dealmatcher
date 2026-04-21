import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/pages/create_update_offer_page.dart';

void main() {
  const double width = 1200;
  const double height = 3000;

  Widget createWidgetUnderTest() {
    return const MaterialApp(home: CreateOfferPage());
  }

  setUp(() {
    final TestWidgetsFlutterBinding binding =
        TestWidgetsFlutterBinding.ensureInitialized();
    binding.platformDispatcher.views.first.physicalSize = const Size(
      width,
      height,
    );
    binding.platformDispatcher.views.first.devicePixelRatio = 1.0;
  });

  tearDown(() {
    final TestWidgetsFlutterBinding binding =
        TestWidgetsFlutterBinding.ensureInitialized();
    binding.platformDispatcher.views.first.resetPhysicalSize();
    binding.platformDispatcher.views.first.resetDevicePixelRatio();
  });

  testWidgets('CreateOfferPage renders correctly and shows initial state', (
    WidgetTester tester,
  ) async {
    final originalOnError = FlutterError.onError;
    FlutterError.onError = (FlutterErrorDetails details) {
      if (details.exceptionAsString().contains('A RenderFlex overflowed')) {
        return;
      }
      originalOnError?.call(details);
    };

    await tester.pumpWidget(createWidgetUnderTest());
    expect(find.text('Add new offer'), findsOneWidget);
    await tester.pumpAndSettle();
    expect(find.text('Choose category'), findsOneWidget);

    FlutterError.onError = originalOnError;
  });

  testWidgets('Form validation works', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final createButton = find.widgetWithText(ElevatedButton, 'Create Offer');
    await tester.tap(createButton);
    await tester.pump();

    expect(find.text('Title is invalid'), findsOneWidget);
    expect(find.text('Category is required'), findsOneWidget);
  });

  testWidgets('Can add and remove tags', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    final tagInput = find.widgetWithText(TextField, 'Add Tag');
    await tester.enterText(tagInput, 'flutter');
    await tester.tap(find.byIcon(Icons.add));
    await tester.pump();

    expect(find.text('flutter'), findsOneWidget);

    await tester.tap(find.byIcon(Icons.cancel).first);
    await tester.pump();
    expect(find.text('flutter'), findsNothing);
  });

  testWidgets('Category selection loads properties', (
    WidgetTester tester,
  ) async {
    final originalOnError = FlutterError.onError;
    FlutterError.onError = (FlutterErrorDetails details) {
      if (details.exceptionAsString().contains('A RenderFlex overflowed')) {
        return;
      }
      originalOnError?.call(details);
    };

    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    await tester.tap(find.text('Choose category'));
    await tester.pumpAndSettle();

    await tester.tap(find.text('Computers').last);
    await tester.pump();
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    await tester.pumpAndSettle();

    expect(find.text('Category Specific Properties'), findsOneWidget);
    expect(find.text('Model'), findsOneWidget);

    FlutterError.onError = originalOnError;
  });

  testWidgets('Submit with valid data shows snackbar', (
    WidgetTester tester,
  ) async {
    final originalOnError = FlutterError.onError;
    FlutterError.onError = (FlutterErrorDetails details) {
      if (details.exceptionAsString().contains('A RenderFlex overflowed')) {
        return;
      }
      originalOnError?.call(details);
    };

    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    // Fill Title
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Title'),
      'Gaming Laptop',
    );
    // Fill Description
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Description'),
      'High end gaming laptop for sale',
    );
    // Fill Price
    await tester.enterText(find.widgetWithText(TextFormField, 'Price'), '5000');
    // Fill Availability
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Availability'),
      '2',
    );

    // Select Category
    await tester.tap(find.text('Choose category'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Computers').last);
    await tester.pumpAndSettle();

    // Fill Category Specific Properties
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Model'),
      'ROG Zephyrus',
    );
    await tester.enterText(
      find.widgetWithText(TextFormField, 'RAM (GB)'),
      '32',
    );
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Storage (GB)'),
      '1000',
    );

    // Select OS
    await tester.tap(find.text('OS'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Windows').last);
    await tester.pumpAndSettle();

    // Toggle Is New (boolean)
    await tester.tap(find.byType(SwitchListTile));
    await tester.pumpAndSettle();

    final createButton = find.widgetWithText(ElevatedButton, 'Create Offer');
    await tester.tap(createButton);

    await tester.pump();
    await tester.pump(const Duration(seconds: 1));

    expect(find.text('Created new offer'), findsOneWidget);

    FlutterError.onError = originalOnError;
  });

  testWidgets('Image picker UI is present', (WidgetTester tester) async {
    await tester.pumpWidget(createWidgetUnderTest());
    await tester.pumpAndSettle();

    expect(find.text('Images'), findsOneWidget);
    expect(find.byIcon(Icons.add_a_photo), findsOneWidget);

    // We cannot easily test the actual picking without mocks,
    // but we can verify it's tappable.
    await tester.tap(find.byIcon(Icons.add_a_photo));
    await tester.pump();
  });
}

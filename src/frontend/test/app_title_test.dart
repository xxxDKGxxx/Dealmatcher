import 'package:flutter_test/flutter_test.dart';

import 'package:frontend/main.dart';

void main() {
  testWidgets('Test app title', (WidgetTester tester) async {
    // Mount DealMatcher app
    await tester.pumpWidget(const DealMatcherApp());

    // Text with app name is expected to be found
    expect(find.text('DealMatcher'), findsWidgets);
  });
}

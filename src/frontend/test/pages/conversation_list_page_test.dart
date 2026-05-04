import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/api/api_conversations.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/pages/conversations_list_page.dart';
import 'package:go_router/go_router.dart';

class MockApiConversations extends ApiConversations {
  @override
  Future<List<ConversationDetail>> getConversations() async {
    final now = DateTime.now();
    return [
      ConversationDetail(
        id: 1,
        offer: Offer(
          id: 101,
          title: 'Testowy iPhone',
          description: 'Opis',
          price: 1000.0,
          images: [],
          seller: const Seller(id: 1, name: 'S1'),
          category: Category(id: 1, name: 'C1', description: 'desc'),
          tags: [],
          properties: {},
          availability: 1,
          status: OfferStatus.active,
          createdAt: now,
          updatedAt: now,
        ),
        buyer: const ConversationParticipant(id: 1, name: 'B1'),
        seller: const ConversationParticipant(id: 2, name: 'S1'),
        lastMessage: 'Halo, czy aktualne?',
        lastMessageAt: now,
        unreadCount: 5,
        status: 'active',
        createdAt: now,
        messages: [],
      ),
    ];
  }
}

void main() {
  Widget createTestableWidget() {
    final router = GoRouter(
      initialLocation: '/',
      routes: [
        GoRoute(
          path: '/',
          builder: (context, state) => ConversationsListPage(api: MockApiConversations()),
        ),
        GoRoute(
          path: '/conversation/:id',
          builder: (context, state) => const Scaffold(body: Text('Conversation Page')),
        ),
      ],
    );

    return MaterialApp.router(
      routerConfig: router,
    );
  }

  testWidgets('Should display title and list of conversations', (WidgetTester tester) async {
    await tester.pumpWidget(createTestableWidget());
    await tester.pump();
    await tester.pumpAndSettle();

    expect(find.text('My Conversations'), findsOneWidget);
    expect(find.text('Testowy iPhone'), findsOneWidget);
  });

  testWidgets('Should show badge with unread messages count', (WidgetTester tester) async {
    await tester.pumpWidget(createTestableWidget());
    await tester.pumpAndSettle();

    expect(find.text('5'), findsOneWidget);
  });

  testWidgets('Should react when clicked a list element', (WidgetTester tester) async {
    await tester.pumpWidget(createTestableWidget());
    await tester.pumpAndSettle();

    final listTile = find.byType(ListTile);
    expect(listTile, findsOneWidget);

    await tester.tap(listTile);
    await tester.pumpAndSettle();

    expect(find.text('Conversation Page'), findsOneWidget);
  });
}
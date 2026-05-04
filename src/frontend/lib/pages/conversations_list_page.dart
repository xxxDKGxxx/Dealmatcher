import 'package:flutter/material.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

class ConversationsListPage extends StatefulWidget {
  const ConversationsListPage({super.key});

  @override
  State<ConversationsListPage> createState() => _ConversationsListPageState();
}

class _ConversationsListPageState extends State<ConversationsListPage> {
  late Future<List<ConversationDetail>> _conversationsFuture;

  @override
  void initState() {
    super.initState();
    _conversationsFuture = _fetchConversations();
  }

  Future<List<ConversationDetail>> _fetchConversations() async {
    // Symulacja opóźnienia sieci
    await Future.delayed(const Duration(milliseconds: 500));

    final now = DateTime.now();

    // Pomocnicza kategoria do mocków
    var mockCategory = Category(
      id: 1,
      name: 'Elektronika',
      description: 'opis',
    );

    return [
      ConversationDetail(
        id: 1,
        offer: Offer(
          id: 101,
          title: 'iPhone 15 Pro Max',
          description: 'Stan idealny, jak nowy.',
          price: 5200.0,
          images: ['https://picsum.photos/200'],
          seller: const Seller(id: 10, name: 'Marek Sprzedawca'),
          category: mockCategory,
          tags: ['apple', 'smartphone'],
          properties: {1: '6.7 cala', 2: '256GB'},
          availability: 1,
          status: OfferStatus.active,
          createdAt: now.subtract(const Duration(days: 2)),
          updatedAt: now.subtract(const Duration(days: 1)),
        ),
        buyer: const ConversationParticipant(id: 1, name: 'Twoje Imię'),
        seller: const ConversationParticipant(id: 10, name: 'Marek Sprzedawca'),
        lastMessage: 'Czy cena podlega jeszcze negocjacji?',
        lastMessageAt: now.subtract(const Duration(minutes: 15)),
        unreadCount: 2,
        status: 'active',
        createdAt: now.subtract(const Duration(days: 1)),
        messages: [],
      ),
      ConversationDetail(
        id: 2,
        offer: Offer(
          id: 102,
          title: 'Klawiatura mechaniczna Keychron',
          description: 'Przełączniki Brown, podświetlenie RGB.',
          price: 450.0,
          images: ['https://picsum.photos/201'],
          seller: const Seller(id: 11, name: 'Anna Kowalska'),
          category: mockCategory,
          tags: ['keyboard', 'tech'],
          properties: {3: 'Bluetooth', 4: 'Mechaniczna'},
          availability: 1,
          status: OfferStatus.active,
          createdAt: now.subtract(const Duration(days: 5)),
          updatedAt: now.subtract(const Duration(days: 5)),
        ),
        buyer: const ConversationParticipant(id: 1, name: 'Twoje Imię'),
        seller: const ConversationParticipant(id: 11, name: 'Anna Kowalska'),
        lastMessage: 'Jasne, mogę wysłać jutro rano.',
        lastMessageAt: now.subtract(const Duration(hours: 2)),
        unreadCount: 0,
        status: 'active',
        createdAt: now.subtract(const Duration(days: 2)),
        messages: [],
      ),
      ConversationDetail(
        id: 3,
        offer: Offer(
          id: 103,
          title: 'Monitor 4K Dell',
          description: 'Matryca IPS, świetne kolory.',
          price: 1800.0,
          images: ['https://picsum.photos/202'],
          seller: const Seller(id: 1, name: 'Twoje Imię'),
          category: mockCategory,
          tags: ['monitor', 'work'],
          properties: {5: '27 cali'},
          availability: 1,
          status: OfferStatus.sold,
          createdAt: now.subtract(const Duration(days: 10)),
          updatedAt: now.subtract(const Duration(days: 1)),
        ),
        buyer: const ConversationParticipant(id: 20, name: 'Piotr Kupujący'),
        seller: const ConversationParticipant(id: 1, name: 'Twoje Imię'),
        lastMessage: 'Dziękuję, monitor dotarł w całości!',
        lastMessageAt: now.subtract(const Duration(days: 1)),
        unreadCount: 0,
        status: 'finished',
        createdAt: now.subtract(const Duration(days: 4)),
        messages: [],
      ),
      ConversationDetail(
        id: 4,
        offer: Offer(
          id: 104,
          title: 'Słuchawki Sony XM5',
          description: 'Najlepsze wyciszenie na rynku.',
          price: 1100.0,
          images: ['https://picsum.photos/203'],
          seller: const Seller(id: 30, name: 'AudioSklep'),
          category: mockCategory,
          tags: ['audio', 'sony'],
          properties: {6: 'ANC'},
          availability: 5,
          status: OfferStatus.active,
          createdAt: now.subtract(const Duration(days: 1)),
          updatedAt: now.subtract(const Duration(hours: 5)),
        ),
        buyer: const ConversationParticipant(id: 1, name: 'Twoje Imię'),
        seller: const ConversationParticipant(id: 30, name: 'AudioSklep'),
        lastMessage: 'Czy mają Państwo kolor biały?',
        lastMessageAt: now.subtract(const Duration(hours: 5)),
        unreadCount: 1,
        status: 'active',
        createdAt: now.subtract(const Duration(hours: 6)),
        messages: [],
      ),
      ConversationDetail(
        id: 5,
        offer: Offer(
          id: 105,
          title: 'MacBook Air M2',
          description: '8GB RAM, 256GB SSD.',
          price: 4200.0,
          images: ['https://picsum.photos/204'],
          seller: const Seller(id: 40, name: 'Krzysztof Laptop'),
          category: mockCategory,
          tags: ['laptop', 'macbook'],
          properties: {7: 'M2 Chip'},
          availability: 1,
          status: OfferStatus.active,
          createdAt: now.subtract(const Duration(days: 15)),
          updatedAt: now.subtract(const Duration(days: 10)),
        ),
        buyer: const ConversationParticipant(id: 1, name: 'Twoje Imię'),
        seller: const ConversationParticipant(id: 40, name: 'Krzysztof Laptop'),
        lastMessage: 'Proponuję 4000 zł i biorę dzisiaj.',
        lastMessageAt: now.subtract(const Duration(days: 3)),
        unreadCount: 0,
        status: 'active',
        createdAt: now.subtract(const Duration(days: 3)),
        messages: [],
      ),
    ];
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: FutureBuilder<List<ConversationDetail>>(
        future: _conversationsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(
              child: Text(
                'Error: ${snapshot.error.toString().trim().replaceFirst('Exception: ', '')}',
              ),
            );
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text('No conversations'));
          }

          final conversations = snapshot.data!;
          final theme = Theme.of(context);

          return Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.only(left: 16, top: 16, bottom: 32),
                child: Text(
                  'My Conversations',
                  style: theme.textTheme.displaySmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              Expanded(
                child: ListView.separated(
                  itemCount: conversations.length,
                  separatorBuilder: (context, index) => const Divider(height: 1),
                  itemBuilder: (context, index) {
                    final conversation = conversations[index];
                    return ListTile(
                      leading: CircleAvatar(
                        child: Text(conversation.offer.title[0].toUpperCase()),
                      ),
                      title: Text(
                        conversation.offer.title,
                        style: const TextStyle(fontWeight: FontWeight.bold),
                      ),
                      subtitle: Text(
                        conversation.lastMessage,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      trailing: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.end,
                        children: [
                          Text(
                            DateFormat('HH:mm').format(conversation.lastMessageAt),
                            style: Theme.of(context).textTheme.bodySmall,
                          ),
                          if (conversation.unreadCount > 0)
                            Container(
                              margin: const EdgeInsets.only(top: 4),
                              padding: const EdgeInsets.all(6),
                              decoration: const BoxDecoration(
                                color: Colors.blue,
                                shape: BoxShape.circle,
                              ),
                              child: Text(
                                '${conversation.unreadCount}',
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 10,
                                ),
                              ),
                            ),
                        ],
                      ),
                      onTap: () {
                        context.push('/conversation/${conversation.id}');
                      },
                    );
                  },
                ),
              ),
            ],
          );
        },
      ),
    );
  }
}
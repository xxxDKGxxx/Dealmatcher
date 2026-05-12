import 'package:flutter/material.dart';
import 'package:frontend/api/api_conversations.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/placeholder_image_widget.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

class ConversationsListPage extends StatefulWidget {
  const ConversationsListPage({super.key, this.api});

  final ApiConversations? api;

  @override
  State<ConversationsListPage> createState() => _ConversationsListPageState();
}

class _ConversationsListPageState extends State<ConversationsListPage> {
  late Future<List<ConversationDetail>> _conversationsFuture;

  late final ApiConversations _apiConversations;

  @override
  void initState() {
    super.initState();
    _apiConversations = widget.api ?? ApiConversations();
    _conversationsFuture = _fetchConversations();
  }

  Future<List<ConversationDetail>> _fetchConversations() async {
    final conversations = await _apiConversations.getConversations();
    return conversations;
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
                  separatorBuilder: (context, index) =>
                      const Divider(height: 1),
                  itemBuilder: (context, index) {
                    final conversation = conversations[index];
                    return ListTile(
                      leading: ClipRRect(
                        borderRadius: BorderRadius.circular(8),
                        child: SizedBox(
                          width: 50,
                          height: 50,
                          child: conversation.offer.images.isNotEmpty
                              ? Image.network(
                                  conversation.offer.images.first,
                                  fit: BoxFit.cover,
                                  errorBuilder: (context, error, stackTrace) =>
                                      placeholderImageWidget(),
                                )
                              : placeholderImageWidget(),
                        ),
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
                            DateFormat(
                              'HH:mm',
                            ).format(conversation.lastMessageAt),
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

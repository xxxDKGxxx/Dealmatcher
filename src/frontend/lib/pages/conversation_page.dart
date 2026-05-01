import 'package:flutter/material.dart';
import 'package:frontend/api/api_conversations.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/conversation.dart';
import 'package:frontend/models/message.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:go_router/go_router.dart';

class ConversationPage extends StatefulWidget {
  final int offerId;

  const ConversationPage({super.key, required this.offerId});

  @override
  State<ConversationPage> createState() => _ConversationPageState();
}

class _ConversationPageState extends State<ConversationPage> {
  final ApiConversations _apiConversations = ApiConversations();
  final ApiProfile _apiProfile = ApiProfile();
  final TextEditingController _messageController = TextEditingController();
  final ScrollController _scrollController = ScrollController();

  late Future<(ConversationDetail?, User)> _dataFuture;

  int? conversationId;
  late Offer? conversationOffer;

  @override
  void initState() {
    super.initState();
    _dataFuture = _fetchData();
  }

  Future<(ConversationDetail?, User)> _fetchData() async {
    final conversations = await ApiConversations().getConversations();
    final offerConversations = conversations.where(
      (c) => c.offer.id == widget.offerId,
    );
    ConversationDetail? conversation = offerConversations.isNotEmpty
        ? offerConversations.first
        : null;
    if (conversation == null) {
      conversation = null;
      conversationOffer = await ApiOffers().getOffer(widget.offerId);
    } else {
      conversationId = conversation.id;
    }

    final currentUser = await _apiProfile.getProfile();
    return (conversation, currentUser);
  }

  void _sendMessage() async {
    if (_messageController.text.trim().isEmpty) return;

    final content = _messageController.text.trim();
    _messageController.clear();

    try {
      if (conversationId == null) {
        conversationId = await _apiConversations.createConversation(
          widget.offerId,
          content,
        );
      } else {
        await _apiConversations.sendMessage(conversationId!, content);
      }
      setState(() {
        _dataFuture = _fetchData();
      });
      _scrollToBottom();
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              "Failed to send message: ${e.toString().trim().replaceFirst('Exception: ', '')}",
            ),
          ),
        );
        context.pop();
      }
    }
  }

  void _scrollToBottom() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (_scrollController.hasClients) {
        _scrollController.animateTo(
          _scrollController.position.maxScrollExtent,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeOut,
        );
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const DealmatcherAppBar(),
      body: FutureBuilder<(ConversationDetail?, User)>(
        future: _dataFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          }
          if (!snapshot.hasData) {
            return const Center(child: Text('Conversation not found'));
          }

          final conversation = snapshot.data!.$1;
          final currentUser = snapshot.data!.$2;

          return Column(
            children: [
              _buildOfferHeader(conversation),
              Expanded(
                child: conversation == null
                    ? SizedBox()
                    : ListView.builder(
                        controller: _scrollController,
                        padding: const EdgeInsets.all(16.0),
                        itemCount: conversation.messages.length,
                        itemBuilder: (context, index) {
                          final message = conversation.messages[index];
                          return _buildMessageBubble(
                            message,
                            message.senderId == currentUser.id,
                          );
                        },
                      ),
              ),
              _buildMessageInput(),
            ],
          );
        },
      ),
    );
  }

  Widget _buildOfferHeader(ConversationDetail? conversation) {
    final theme = Theme.of(context);
    final offer = conversation != null
        ? conversation.offer
        : conversationOffer!;

    return Container(
      padding: const EdgeInsets.all(12.0),
      decoration: BoxDecoration(
        color: theme.colorScheme.surfaceContainerHighest,
        border: Border(
          bottom: BorderSide(color: theme.dividerColor, width: 0.5),
        ),
      ),
      child: Row(
        children: [
          if (offer.images.isNotEmpty)
            ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: Image.network(
                offer.images.first,
                width: 50,
                height: 50,
                fit: BoxFit.cover,
              ),
            )
          else
            Container(
              width: 50,
              height: 50,
              decoration: BoxDecoration(
                color: Colors.grey[300],
                borderRadius: BorderRadius.circular(4),
              ),
              child: const Icon(Icons.image, color: Colors.grey),
            ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  offer.title,
                  style: const TextStyle(fontWeight: FontWeight.bold),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                Text(
                  "${offer.price.toStringAsFixed(2)} zł",
                  style: TextStyle(
                    color: theme.colorScheme.primary,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          ),
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                "Status: ${offer.status.name.toUpperCase()}",
                style: theme.textTheme.bodySmall,
              ),
              IconButton(
                onPressed: () {
                  if (mounted) {
                    context.go('/offer/${offer.id}');
                  }
                },
                icon: const Icon(Icons.chevron_right),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildMessageBubble(Message message, bool isMe) {
    final theme = Theme.of(context);

    return Align(
      alignment: isMe ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4.0),
        padding: const EdgeInsets.symmetric(horizontal: 12.0, vertical: 8.0),
        decoration: BoxDecoration(
          color: isMe
              ? theme.colorScheme.primary
              : theme.colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(12),
            topRight: const Radius.circular(12),
            bottomLeft: Radius.circular(isMe ? 12 : 0),
            bottomRight: Radius.circular(isMe ? 0 : 12),
          ),
        ),
        constraints: BoxConstraints(
          maxWidth: MediaQuery.of(context).size.width * 0.7,
        ),
        child: Column(
          crossAxisAlignment: isMe
              ? CrossAxisAlignment.end
              : CrossAxisAlignment.start,
          children: [
            Text(
              message.content,
              style: TextStyle(
                color: isMe
                    ? theme.colorScheme.onPrimary
                    : theme.colorScheme.onSurfaceVariant,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              _formatTime(message.createdAt),
              style: TextStyle(
                fontSize: 10,
                color: isMe
                    ? theme.colorScheme.onPrimary.withValues(alpha: 0.7)
                    : theme.colorScheme.onSurfaceVariant.withValues(alpha: 0.7),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMessageInput() {
    final theme = Theme.of(context);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8.0, vertical: 8.0),
      decoration: BoxDecoration(
        color: theme.colorScheme.surface,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 5,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: SafeArea(
        child: Row(
          children: [
            Expanded(
              child: TextField(
                controller: _messageController,
                decoration: InputDecoration(
                  hintText: "Type a message...",
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(24),
                    borderSide: BorderSide.none,
                  ),
                  filled: true,
                  contentPadding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 8,
                  ),
                ),
                textInputAction: TextInputAction.send,
                onSubmitted: (_) => _sendMessage(),
              ),
            ),
            const SizedBox(width: 8),
            IconButton(
              onPressed: _sendMessage,
              icon: Icon(Icons.send, color: theme.colorScheme.primary),
            ),
          ],
        ),
      ),
    );
  }

  String _formatTime(DateTime date) {
    return "${date.hour.toString().padLeft(2, '0')}:${date.minute.toString().padLeft(2, '0')}";
  }
}

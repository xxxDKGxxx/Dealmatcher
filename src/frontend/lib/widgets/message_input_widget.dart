import 'package:flutter/material.dart';

Widget buildMessageInput(
  BuildContext context, {
  required TextEditingController messageController,
  required void Function() onSubmit,
}) {
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
              controller: messageController,
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
              onSubmitted: (_) => onSubmit(),
            ),
          ),
          const SizedBox(width: 8),
          IconButton(
            onPressed: onSubmit,
            icon: Icon(Icons.send, color: theme.colorScheme.primary),
          ),
        ],
      ),
    ),
  );
}

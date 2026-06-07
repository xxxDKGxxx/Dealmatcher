import 'package:flutter/material.dart';
import 'package:frontend/models/ban.dart';

Widget banListTile({required Ban ban, required ThemeData theme}) {
  return Card(
    child: SizedBox(
      height: 120,
      child: Row(
        children: [
          Expanded(child: Icon(Icons.block_rounded)),
          const SizedBox(width: 12),
          Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'User ID ${ban.userId}; Reason: ${ban.reason}',
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Issued by admin of ID: ${ban.issuedBy} at ${ban.issuedAt}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Expires at: ${ban.expiresAt}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Is active: ${ban.isActive}',
                    style: theme.textTheme.bodyMedium,
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    ),
  );
}

import 'package:flutter/material.dart';
import 'package:frontend/models/ban.dart';

Widget banListTile({required Ban ban, required ThemeData theme}) {
  return Card(
    child: ConstrainedBox(
      constraints: BoxConstraints(minHeight: 120),
      child: Row(
        children: [
          const SizedBox(width: 16),
          SizedBox(height: 60, child: FittedBox(fit: BoxFit.contain, child: Icon(Icons.block_rounded))),
          const SizedBox(width: 16),
          Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'User ID ${ban.userId}',
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Reason: ${ban.reason}',
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Issued by admin of ID: ${ban.issuedBy} at ${ban.issuedAt}',
                    style: theme.textTheme.bodyMedium
                  ),
                  Text(
                    'Expires at: ${ban.expiresAt}',
                    style: theme.textTheme.bodyMedium
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

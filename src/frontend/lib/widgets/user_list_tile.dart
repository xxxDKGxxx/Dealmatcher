import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/ban_user_form_widget.dart';

Widget userListTile({
  required User user,
  required ThemeData theme,
  required BuildContext context,
}) {
  return Card(
    child: SizedBox(
      height: 120,
      child: Row(
        children: [
          Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    '${user.name} ${user.surname}',
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'id: ${user.id}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'E-mail: ${user.email}',
                    style: theme.textTheme.bodyMedium,
                  ),
                  Text(
                    'Status: ${user.status.toString().toUpperCase().split('.').last}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
          ),
          TextButton.icon(
            onPressed: () {
              showDialog(
                context: context,
                builder: (context) {
                  return Dialog(
                    child: BanUserFormWidget(
                      userId: user.id,
                      onSubmit: (userId, reason, expiresAt) async {
                        await ApiAdmin().banUser(userId, reason, expiresAt);
                      },
                    ),
                  );
                },
              );
            },
            label: Text('Ban'),
            icon: Icon(Icons.block_rounded),
          ),
        ],
      ),
    ),
  );
}

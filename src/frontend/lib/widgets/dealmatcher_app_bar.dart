import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class DealmatcherAppBar extends StatelessWidget implements PreferredSizeWidget {
  const DealmatcherAppBar({super.key, this.actions});

  final List<Widget>? actions;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppBar(
      title: TextButton(
        child: Text('DealMatcher', style: theme.textTheme.titleLarge),
        onPressed: () => context.go('/'),
      ),
      backgroundColor: theme.colorScheme.inversePrimary,
      actions: actions,
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);
}

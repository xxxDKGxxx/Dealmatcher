import 'package:flutter/material.dart';
import 'package:frontend/api/api_core.dart';
import 'package:go_router/go_router.dart';

class MenuDrawer extends StatelessWidget {
  MenuDrawer({super.key});

  final redColor = Colors.red.shade700;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final themeColor = theme.colorScheme.inversePrimary;
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          DrawerHeader(
            decoration: BoxDecoration(color: themeColor),
            child: Text(
              'Menu',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.w700),
            ),
          ),
          // My offers page
          ListTile(
            leading: Icon(Icons.local_offer),
            title: Text('My Offers'),
            onTap: () {
              context.go('/my-offers');
            },
          ),
          // Profile Page
          ListTile(
            leading: Icon(Icons.person),
            title: Text('Profile'),
            onTap: () {
              context.go('/profile');
            },
          ),

          // Log out button
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: Divider(height: 1),
          ),
          ListTile(
            leading: Icon(Icons.exit_to_app_rounded, color: redColor),
            title: Text('Log out', style: TextStyle(color: redColor)),
            onTap: () {
              ApiCore().nullToken();
              context.go('/login');
              if (context.mounted) {
                ScaffoldMessenger.of(
                  context,
                ).showSnackBar(SnackBar(content: Text('Logged out')));
              }
            },
          ),
        ],
      ),
    );
  }
}

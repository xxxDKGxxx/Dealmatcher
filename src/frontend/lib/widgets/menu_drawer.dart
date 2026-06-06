import 'package:flutter/material.dart';
import 'package:frontend/api/api_auth.dart';
import 'package:frontend/api/api_profile.dart';
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
          // My conversations page
          ListTile(
            leading: Icon(Icons.chat),
            title: Text('My Conversations'),
            onTap: () {
              context.go('/conversations');
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

          FutureBuilder(
            future: ApiProfile().getProfile(),
            builder: (context, snapshot) {
              if (!snapshot.hasError &&
                  snapshot
                      .hasData //&& snapshot.data!.status.name.toString().trim().toLowerCase() == 'admin'
                      ) {
                return Column(
                  children: [
                    const Padding(
                      padding: EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 8,
                      ),
                      child: Divider(height: 1),
                    ),

                    ListTile(
                      leading: Icon(Icons.local_offer_rounded),
                      title: Text('Admin'),
                      onTap: () {
                        context.go('/admin');
                      },
                    ),
                  ],
                );
              }

              return SizedBox(height: 0);
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
            onTap: () async {
              await ApiAuth().logout();
              if (context.mounted) {
                context.go('/login');
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

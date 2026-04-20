import 'package:flutter/material.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/menu_drawer.dart';
import 'package:go_router/go_router.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(
        actions: [
          IconButton(
            icon: Icon(Icons.add_circle_outline),
            tooltip: 'Add offer',
            onPressed: () {
              context.go('/add-offer');
            },
          ),
        ],
      ),
      drawer: MenuDrawer(),
      //body: Center(child: Text('Welcome to Home Page')),
      body: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          TextButton(
            onPressed: () => context.go('/add-offer'),
            child: const Text('create'),
          ),
          TextButton(
            onPressed: () => context.go('/update-offer'),
            child: const Text('no id'),
          ),
          TextButton(
            onPressed: () => context.go('/update-offer/0'),
            child: const Text('update id 0'),
          ),
        ],
      ),
    );
  }
}

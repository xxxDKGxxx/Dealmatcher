import 'package:flutter/material.dart';
<<<<<<< HEAD
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/menu_drawer.dart';
import 'package:go_router/go_router.dart';
=======
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
<<<<<<< HEAD
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
      body: Center(child: Text('Welcome to Home Page')),
=======
    var theme = Theme.of(context);
    return Scaffold(
      appBar: AppBar(
        title: Text('DealMatcher'),
        backgroundColor: theme.colorScheme.inversePrimary,
      ),
      body: ColoredBox(color: Colors.white),
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
    );
  }
}

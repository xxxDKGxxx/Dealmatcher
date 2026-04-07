import 'package:flutter/material.dart';
import 'package:frontend/widgets/menu_drawer.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final themeColor = theme.colorScheme.inversePrimary;
    return Scaffold(
      appBar: AppBar(title: Text('DealMatcher'), backgroundColor: themeColor),
      drawer: MenuDrawer(),
      body: Center(child: Text('Welcome to Home Page')),
    );
  }
}

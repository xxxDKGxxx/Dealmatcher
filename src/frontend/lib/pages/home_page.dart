import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    var theme = Theme.of(context);
    return Scaffold(
      appBar: AppBar(
        title: Text('DealMatcher'),
        backgroundColor: theme.colorScheme.inversePrimary,
        actions: [
          IconButton(
            icon: Icon(Icons.add_circle_outline),
            tooltip: 'Add offer',
            onPressed: () {
              context.push('/add-offer');
            },
          ),
        ],
      ),
      body: Center(child: Text('Welcome to Home Page')),
    );
  }
}

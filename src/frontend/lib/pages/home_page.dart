import 'package:flutter/material.dart';
import 'package:frontend/pages/offers_swiping_page.dart';
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
            icon: Icon(Icons.shopping_cart_rounded),
            tooltip: 'Show cart',
            onPressed: () {
              context.go('/cart');
            },
          ),
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
      body: OffersSwipingPage(),
    );
  }
}

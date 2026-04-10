import 'package:flutter/material.dart';
import 'package:frontend/router/go_router.dart';

void main() {
  runApp(const DealMatcherApp());
}

class DealMatcherApp extends StatelessWidget {
  const DealMatcherApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'DealMatcher',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.orange),
      ),
      routerConfig: globalRouter,
    );
  }
}

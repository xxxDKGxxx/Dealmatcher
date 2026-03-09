import 'package:flutter/material.dart';
import 'package:frontend/pages/home_page.dart';

void main() {
  runApp(const DealMatcherApp());
}

class DealMatcherApp extends StatelessWidget {
  const DealMatcherApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'DealMatcher',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.orange),
      ),
      home: const HomePage(),
    );
  }
}

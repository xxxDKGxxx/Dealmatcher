import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:frontend/router/go_router.dart';

class DealMatcherApp extends StatelessWidget {
  const DealMatcherApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'DealMatcher',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.orange),
      ),
      scrollBehavior: const MaterialScrollBehavior().copyWith(
        dragDevices: {
          PointerDeviceKind.mouse,
          PointerDeviceKind.touch,
          PointerDeviceKind.stylus,
          PointerDeviceKind.trackpad,
        },
      ),
      routerConfig: globalRouter,
    );
  }
}

import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:frontend/router/go_router.dart';

class DealMatcherApp extends StatelessWidget {
  const DealMatcherApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'DealMatcher',
      theme: ThemeData.dark().copyWith(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.purple,
          brightness: Brightness.dark,
        ),
        scaffoldBackgroundColor: const Color(0xFF121212),
        drawerTheme: const DrawerThemeData(backgroundColor: Color(0xFF1E1E1E)),
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

import 'package:flutter/material.dart';
import 'package:frontend/api/api_core.dart';
import 'package:frontend/dealmatcher_app.dart';

void main() {
  ApiCore().init('https://dev.dealmatcher.com');
  runApp(const DealMatcherApp());
}

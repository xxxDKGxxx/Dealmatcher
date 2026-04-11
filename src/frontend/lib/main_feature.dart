import 'package:flutter/material.dart';
import 'package:frontend/api/api_core.dart';
import 'package:frontend/dealmatcher_app.dart';

void main() {
  ApiCore().init('http://localhost:5000');
  runApp(const DealMatcherApp());
}

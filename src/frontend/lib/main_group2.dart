import 'package:flutter/material.dart';
import 'package:frontend/api/api_core.dart';
import 'package:frontend/dealmatcher_app.dart';

void main() {
  ApiCore().init(
    'https://dealmatcher-backend.redsmoke-09dcd534.germanywestcentral.azurecontainerapps.io/',
  );
  runApp(const DealMatcherApp());
}

import 'package:flutter/material.dart';

Widget placeholderImageWidget({double size = 40}) => Container(
  color: Colors.grey[200],
  alignment: Alignment.center,
  child: Icon(Icons.image_not_supported, color: Colors.grey, size: size),
);

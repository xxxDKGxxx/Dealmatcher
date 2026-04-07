import 'package:flutter/material.dart';

class DisplayField extends StatelessWidget {
  const DisplayField({super.key, required this.label, required this.text});

  final String label;
  final String text;

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('$label:', style: textTheme.labelMedium),
        Padding(
          padding: EdgeInsets.only(left: 8),
          child: Text(text, style: textTheme.titleLarge),
        ),
        SizedBox(height: 4),
        Padding(
          padding: EdgeInsets.symmetric(horizontal: 16),
          child: Divider(height: 1),
        ),
        SizedBox(height: 12),
      ],
    );
  }
}
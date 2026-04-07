import 'package:flutter/material.dart';
import '../../models/property.dart';
import 'form_fields.dart';

class PropertyField extends StatelessWidget {
  final Property property;
  final dynamic value;
  final ValueChanged<dynamic> onChanged;

  const PropertyField({
    super.key,
    required this.property,
    required this.value,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    Widget field;

    switch (property.type) {
      case PropertyType.number:
        field = numberFormField(
          text: property.name,
          initialValue: value?.toString(),
          onChanged: (newValue) {
            onChanged(double.tryParse(newValue) ?? 0.0);
          },
        );
      case PropertyType.boolean:
        field = switchFormField(
          text: property.name,
          value: value ?? false,
          onChanged: (newValue) {
            onChanged(newValue);
          },
        );
      case PropertyType.select:
        field = dropdownFormField<String>(
          text: property.name,
          value: value,
          items: property.options
              .map((opt) => DropdownMenuItem(value: opt, child: Text(opt)))
              .toList(),
          onChanged: (newValue) {
            onChanged(newValue);
          },
          validator: (val) => (val == null || val.isEmpty)
              ? "${property.name} is required"
              : null,
        );
      case PropertyType.text:
        field = nonEmptyTextFormField(
          text: property.name,
          initialValue: value,
          onChanged: (newValue) {
            onChanged(newValue);
          },
        );
    }

    return Padding(padding: const EdgeInsets.only(bottom: 16.0), child: field);
  }
}

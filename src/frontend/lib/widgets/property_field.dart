import 'package:flutter/material.dart';
import '../models/property_definition.dart';
import 'form_fields.dart';

class PropertyField extends StatelessWidget {
  final PropertyDefinition property;
  final dynamic value;
  final ValueChanged<dynamic> onChanged;
  final bool enabled;
  final Widget? suffixIcon;

  const PropertyField({
    super.key,
    required this.property,
    required this.value,
    required this.onChanged,
    this.enabled = true,
    this.suffixIcon,
  });

  @override
  Widget build(BuildContext context) {
    Widget field;

    switch (property.type) {
      case PropertyType.numeric:
        field = numberFormField(
          text: property.name,
          initialValue: value?.toString(),
          onChanged: (newValue) {
            onChanged(double.tryParse(newValue) ?? 0.0);
          },
          enabled: enabled,
        );
      case PropertyType.boolean:
        field = switchFormField(
          text: property.name,
          value: value is bool ? value : (value?.toString() == 'true'),
          onChanged: enabled
              ? (newValue) {
                  onChanged(newValue);
                }
              : null,
          enabled: enabled,
        );
      case PropertyType.select:
        field = dropdownFormField<String>(
          text: property.name,
          value: value,
          items: property.options
              .map((opt) => DropdownMenuItem(value: opt, child: Text(opt)))
              .toList(),
          onChanged: enabled
              ? (newValue) {
                  if (newValue != null) onChanged(newValue);
                }
              : null,
          validator: (val) => (val == null || val.isEmpty)
              ? "${property.name} is required"
              : null,
          enabled: enabled,
        );
      case PropertyType.text:
        field = nonEmptyTextFormField(
          text: property.name,
          initialValue: value?.toString(),
          onChanged: (newValue) {
            onChanged(newValue);
          },
          enabled: enabled,
        );
    }

    return Padding(
      padding: const EdgeInsets.only(bottom: 16.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Expanded(child: field),
          if (suffixIcon != null) suffixIcon!,
        ],
      ),
    );
  }
}

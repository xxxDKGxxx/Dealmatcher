import re

# Fix create_update_offer_page.dart
with open("src/frontend/lib/pages/create_update_offer_page.dart", "r") as f:
    content = f.read()

# Replace Title field
title_old = """                          nonEmptyTextFormField(
                            controller: _titleController,
                            text: "Title",
                            enabled: !isUpdated || _isTitleEditing,
                            suffixIcon: _buildEditIcon(
                              isUpdated,
                              _isTitleEditing,
                              () {
                                setState(
                                  () => _isTitleEditing = !_isTitleEditing,
                                );
                              },
                            ),
                          ),"""
title_new = """                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                child: nonEmptyTextFormField(
                                  controller: _titleController,
                                  text: "Title",
                                  enabled: !isUpdated || _isTitleEditing,
                                ),
                              ),
                              if (isUpdated)
                                _buildEditIcon(
                                  isUpdated,
                                  _isTitleEditing,
                                  () {
                                    setState(
                                      () => _isTitleEditing = !_isTitleEditing,
                                    );
                                  },
                                )!,
                            ],
                          ),"""
content = content.replace(title_old, title_new)

# Replace Description field
desc_old = """                          nonEmptyTextFormField(
                            controller: _descriptionController,
                            text: "Description",
                            maxLines: 4,
                            enabled: !isUpdated || _isDescriptionEditing,
                            suffixIcon: _buildEditIcon(
                              isUpdated,
                              _isDescriptionEditing,
                              () {
                                setState(
                                  () => _isDescriptionEditing =
                                      !_isDescriptionEditing,
                                );
                              },
                            ),
                          ),"""
desc_new = """                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                child: nonEmptyTextFormField(
                                  controller: _descriptionController,
                                  text: "Description",
                                  maxLines: 4,
                                  enabled: !isUpdated || _isDescriptionEditing,
                                ),
                              ),
                              if (isUpdated)
                                _buildEditIcon(
                                  isUpdated,
                                  _isDescriptionEditing,
                                  () {
                                    setState(
                                      () => _isDescriptionEditing =
                                          !_isDescriptionEditing,
                                    );
                                  },
                                )!,
                            ],
                          ),"""
content = content.replace(desc_old, desc_new)

# Replace Price and Availability
price_old = """                          Row(
                            children: [
                              Expanded(
                                child: numberFormField(
                                  controller: _priceController,
                                  text: "Price",
                                  enabled: !isUpdated || _isPriceEditing,
                                  suffixIcon: _buildEditIcon(
                                    isUpdated,
                                    _isPriceEditing,
                                    () {
                                      setState(
                                        () =>
                                            _isPriceEditing = !_isPriceEditing,
                                      );
                                    },
                                  ),
                                ),
                              ),
                              const SizedBox(width: 16),
                              Expanded(
                                child: numberFormField(
                                  controller: _availabilityController,
                                  text: "Availability",
                                  enabled: !isUpdated || _isAvailabilityEditing,
                                  suffixIcon: _buildEditIcon(
                                    isUpdated,
                                    _isAvailabilityEditing,
                                    () {
                                      setState(
                                        () => _isAvailabilityEditing =
                                            !_isAvailabilityEditing,
                                      );
                                    },
                                  ),
                                ),
                              ),
                            ],
                          ),"""
price_new = """                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                child: numberFormField(
                                  controller: _priceController,
                                  text: "Price",
                                  enabled: !isUpdated || _isPriceEditing,
                                ),
                              ),
                              if (isUpdated)
                                _buildEditIcon(
                                  isUpdated,
                                  _isPriceEditing,
                                  () {
                                    setState(
                                      () =>
                                          _isPriceEditing = !_isPriceEditing,
                                    );
                                  },
                                )!,
                              const SizedBox(width: 16),
                              Expanded(
                                child: numberFormField(
                                  controller: _availabilityController,
                                  text: "Availability",
                                  enabled: !isUpdated || _isAvailabilityEditing,
                                ),
                              ),
                              if (isUpdated)
                                _buildEditIcon(
                                  isUpdated,
                                  _isAvailabilityEditing,
                                  () {
                                    setState(
                                      () => _isAvailabilityEditing =
                                          !_isAvailabilityEditing,
                                    );
                                  },
                                )!,
                            ],
                          ),"""
content = content.replace(price_old, price_new)

with open("src/frontend/lib/pages/create_update_offer_page.dart", "w") as f:
    f.write(content)

# Fix property_field.dart
prop_code = """import 'package:flutter/material.dart';
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
          onChanged: enabled ? (newValue) {
            onChanged(newValue);
          } : null,
        );
      case PropertyType.select:
        field = dropdownFormField<String>(
          text: property.name,
          value: value,
          items: property.options
              .map((opt) => DropdownMenuItem(value: opt, child: Text(opt)))
              .toList(),
          onChanged: enabled ? (newValue) {
            if (newValue != null) onChanged(newValue);
          } : null,
          validator: (val) => (val == null || val.isEmpty)
              ? "${property.name} is required"
              : null,
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
"""

with open("src/frontend/lib/widgets/property_field.dart", "w") as f:
    f.write(prop_code)


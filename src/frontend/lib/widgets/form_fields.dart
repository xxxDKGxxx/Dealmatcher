import 'package:flutter/material.dart';

Widget nonEmptyTextFormField({
  TextEditingController? controller,
  required String text,
  bool Function(String)? additionalValidator,
  bool obscureText = false,
  String? errorText,
  int? maxLines = 1,
  void Function(String)? onChanged,
  String? initialValue,
  bool? enabled,
  Widget? suffixIcon,
}) => TextFormField(
  controller: controller,
  initialValue: initialValue,
  obscureText: obscureText,
  maxLines: maxLines,
  onChanged: onChanged,
  readOnly: !(enabled ?? true),
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
    suffixIcon: suffixIcon,
    filled: !(enabled ?? true),
  ),
  validator: (value) {
    if (value == null ||
        value.trim().isEmpty ||
        (additionalValidator != null && additionalValidator(value))) {
      return errorText ?? "$text is invalid";
    }
    return null;
  },
);

Widget numberFormField({
  TextEditingController? controller,
  required String text,
  TextInputType keyboardType = TextInputType.number,
  void Function(String)? onChanged,
  String? initialValue,
  String? Function(String?)? validator,
  bool? enabled,
  Widget? suffixIcon,
}) => TextFormField(
  controller: controller,
  initialValue: initialValue,
  keyboardType: keyboardType,
  onChanged: onChanged,
  readOnly: !(enabled ?? true),
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
    suffixIcon: suffixIcon,
    filled: !(enabled ?? true),
  ),
  validator:
      validator ??
      (value) {
        if (value == null || value.trim().isEmpty) {
          return "$text is empty";
        }
        if (double.tryParse(value) == null) {
          return "$text must be a number";
        }
        return null;
      },
);

Widget emailFormField({required TextEditingController controller}) =>
    nonEmptyTextFormField(
      controller: controller,
      text: 'Email',
      additionalValidator: (s) => !RegExp(
        r"^[a-zA-Z0-9.a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9]+\.[a-zA-Z]+",
      ).hasMatch(s),
    );

Widget passwordFormField({required TextEditingController controller}) =>
    nonEmptyTextFormField(
      controller: controller,
      text: 'Password',
      obscureText: true,
      additionalValidator: (s) => s.length < 6,
      errorText: 'Password must be at least 6 characters long',
    );

Widget dropdownFormField<T>({
  required String text,
  required List<DropdownMenuItem<T>> items,
  required T? value,
  void Function(T?)? onChanged,
  String? Function(T?)? validator,
  Widget? icon,
  bool isExpanded = true,
  String Function(T value)? itemLabelBuilder,
  bool? enabled,
}) => DropdownButtonFormField<T>(
  value: value,
  items: items,
  onChanged: (enabled ?? true) ? onChanged : null,
  isExpanded: isExpanded,
  icon: icon,
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
    filled: !(enabled ?? true),
  ),
  selectedItemBuilder: (context) {
    return items.map((item) {
      final val = item.value;

      return Text(
        val != null
            ? (itemLabelBuilder != null
                  ? itemLabelBuilder(val)
                  : val.toString())
            : '',
        overflow: TextOverflow.ellipsis,
      );
    }).toList();
  },
  validator:
      validator ??
      (value) {
        if (value == null) {
          return "$text is required";
        }
        return null;
      },
);

Widget switchFormField({
  required String text,
  required bool value,
  void Function(bool)? onChanged,
  bool? enabled,
}) =>
    InputDecorator(
      decoration: InputDecoration(
        labelText: text,
        border: const OutlineInputBorder(),
        filled: !(enabled ?? true),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(value ? "Yes" : "No"),
          SizedBox(
            height: 24,
            child: Switch(
              value: value,
              onChanged: (enabled ?? true) ? onChanged : null,
            ),
          ),
        ],
      ),
    );

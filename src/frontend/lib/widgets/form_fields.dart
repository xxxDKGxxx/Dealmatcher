import 'package:flutter/material.dart';

Widget nonEmptyTextFormField({
<<<<<<< HEAD
  TextEditingController? controller,
=======
  required TextEditingController controller,
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
  required String text,
  bool Function(String)? additionalValidator,
  bool obscureText = false,
  String? errorText,
<<<<<<< HEAD
  int? maxLines = 1,
  void Function(String)? onChanged,
  String? initialValue,
}) => TextFormField(
  controller: controller,
  initialValue: initialValue,
  obscureText: obscureText,
  maxLines: maxLines,
  onChanged: onChanged,
=======
}) => TextFormField(
  controller: controller,
  obscureText: obscureText,
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
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
<<<<<<< HEAD
  TextEditingController? controller,
  required String text,
  TextInputType keyboardType = TextInputType.number,
  void Function(String)? onChanged,
  String? initialValue,
  String? Function(String?)? validator,
}) => TextFormField(
  controller: controller,
  initialValue: initialValue,
  keyboardType: keyboardType,
  onChanged: onChanged,
=======
  required TextEditingController controller,
  required String text,
}) => TextFormField(
  controller: controller,
  readOnly: true,
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
  ),
<<<<<<< HEAD
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
=======
  validator: (value) {
    if (value == null || value.trim().isEmpty) {
      return "$text is empty";
    }
    return null;
  },
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
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
<<<<<<< HEAD

Widget dropdownFormField<T>({
  required String text,
  required List<DropdownMenuItem<T>> items,
  required T? value,
  required void Function(T?) onChanged,
  String? Function(T?)? validator,
  Widget? icon,
  bool isExpanded = true,
}) => DropdownButtonFormField<T>(
  initialValue: value,
  items: items,
  onChanged: onChanged,
  isExpanded: isExpanded,
  icon: icon,
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
  ),
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
  required void Function(bool) onChanged,
}) => SwitchListTile(title: Text(text), value: value, onChanged: onChanged);
=======
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de

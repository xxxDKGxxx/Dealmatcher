import 'package:flutter/material.dart';

Widget nonEmptyTextFormField({
  required TextEditingController controller,
  required String text,
  bool Function(String)? additionalValidator,
  bool obscureText = false,
  String? errorText,
}) => TextFormField(
  controller: controller,
  obscureText: obscureText,
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
  required TextEditingController controller,
  required String text,
}) => TextFormField(
  controller: controller,
  readOnly: true,
  decoration: InputDecoration(
    labelText: text,
    border: const OutlineInputBorder(),
  ),
  validator: (value) {
    if (value == null || value.trim().isEmpty) {
      return "$text is empty";
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

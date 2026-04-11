import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class ProfileEditRequest extends RequestModel {
  const ProfileEditRequest({required this.name, required this.surname});

  final String name;
  final String surname;

  @override
  String toJson() {
    return jsonEncode({'name': name, 'surname': surname});
  }
}

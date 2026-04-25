import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/user.dart';

class ProfileResponse extends ResponseModel {
  ProfileResponse({required super.response});

  late User user;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    try {
      user = User.fromJson(data as Map<String, dynamic>);
    } catch (e) {
      throw Exception(
        'User profile response does not contain valid profile data.',
      );
    }
  }
}
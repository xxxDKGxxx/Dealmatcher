import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/user.dart';

class ProfileResponse extends ResponseModel {
  ProfileResponse({required super.response});

  late User user;

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    if (data.containsKey('id') &&
        data['id'] != null &&
        data.containsKey('email') &&
        data['email'] != null &&
        data.containsKey('name') &&
        data['name'] != null &&
        data.containsKey('surname') &&
        data['surname'] != null &&
        data.containsKey('status') &&
        data['status'] != null &&
        data.containsKey('createdAt') &&
        data['createdAt'] != null) {
      user = User(
        id: data['id'],
        email: data['email'],
        name: data['name'],
        surname: data['surname'],
        status: UserStatus.fromString(data['status']),
        createdAt: DateTime.parse(data['createdAt']),
      );
    } else {
      throw Exception(
        'User profile response does not contain valid profile data.',
      );
    }
  }
}

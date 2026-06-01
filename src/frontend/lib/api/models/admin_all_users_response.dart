import 'dart:convert';

import 'package:frontend/api/models/response_model.dart';
import 'package:frontend/models/all_users.dart';

class AdminAllUsersResponse extends ResponseModel {
  late AllUsers allUsers;

  AdminAllUsersResponse({required super.response});

  @override
  void fromJson() {
    final data = jsonDecode(response.body);
    allUsers = AllUsers.fromJson(data);
  }
}

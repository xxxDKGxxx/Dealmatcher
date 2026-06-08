import 'dart:convert';

import 'package:frontend/api/models/request_model.dart';

class AdminUpdateOfferStatusRequest extends RequestModel {
  const AdminUpdateOfferStatusRequest({required this.status});

  final String status;

  @override
  String toJson() {
    return jsonEncode({'status': status});
  }
}

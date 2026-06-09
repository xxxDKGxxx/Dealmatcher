import 'dart:convert';
import 'package:frontend/api/models/request_model.dart';

class OfferChangeStatusRequest extends RequestModel {
  final String status;

  OfferChangeStatusRequest({required this.status});

  @override
  String toJson() {
    return jsonEncode({
      'status': status,
    });
  }
}

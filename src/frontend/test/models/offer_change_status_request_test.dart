import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/api/models/offer_change_status_request.dart';
import 'dart:convert';

void main() {
  group('OfferChangeStatusRequest', () {
    test('toJson returns correct JSON string', () {
      final request = OfferChangeStatusRequest(status: 'SOLD');
      final jsonString = request.toJson();
      final map = jsonDecode(jsonString);

      expect(map['status'], 'SOLD');
    });
  });
}

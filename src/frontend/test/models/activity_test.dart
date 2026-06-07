import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/models/activity.dart';

void main() {
  group('Activity Model Tests', () {
    test('Activity.fromJson should parse correctly', () {
      final json = {
        'id': 1,
        'userId': 2,
        'offerId': 3,
        'action': 'Login',
        'details': {'ip': '127.0.0.1'},
        'ipAddress': '127.0.0.1',
        'createdAt': '2023-01-01T12:00:00.000Z',
      };

      final activity = Activity.fromJson(json);

      expect(activity.id, 1);
      expect(activity.userId, 2);
      expect(activity.offerId, 3);
      expect(activity.action, 'Login');
      expect(activity.details['ip'], '127.0.0.1');
      expect(activity.ipAddress, '127.0.0.1');
      expect(activity.createdAt.year, 2023);
    });

    test('Activity.fromJson should handle null offerId and empty details', () {
      final json = {
        'id': 1,
        'userId': 2,
        'offerId': null,
        'action': 'Logout',
        'details': null,
        'ipAddress': '192.168.1.1',
        'createdAt': '2023-01-02T12:00:00.000Z',
      };

      final activity = Activity.fromJson(json);

      expect(activity.id, 1);
      expect(activity.userId, 2);
      expect(activity.offerId, isNull);
      expect(activity.action, 'Logout');
      expect(activity.details, isEmpty);
      expect(activity.ipAddress, '192.168.1.1');
      expect(activity.createdAt.day, 2);
    });
  });
}

class Activity {
  final int id;
  final int userId;
  final int? offerId;
  final String action;
  final Map<String, dynamic> details;
  final String ipAddress;
  final DateTime createdAt;

  Activity({
    required this.id,
    required this.userId,
    this.offerId,
    required this.action,
    required this.details,
    required this.ipAddress,
    required this.createdAt,
  });

  factory Activity.fromJson(Map<String, dynamic> json) {
    return Activity(
      id: json['id'],
      userId: json['userId'],
      offerId: json['offerId'],
      action: json['action'] ?? '',
      details: json['details'] != null
          ? Map<String, dynamic>.from(json['details'])
          : {},
      ipAddress: json['ipAddress'] ?? '',
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}

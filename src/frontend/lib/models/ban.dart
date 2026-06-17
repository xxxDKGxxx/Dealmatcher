class Ban {
  const Ban({
    required this.id,
    required this.userId,
    required this.reason,
    required this.issuedBy,
    required this.issuedAt,
    required this.expiresAt,
    required this.isActive,
  });

  final int id;
  final int userId;
  final String reason;
  final int issuedBy;
  final DateTime issuedAt;
  final DateTime? expiresAt;
  final bool isActive;

  factory Ban.fromJson(Map<String, dynamic> json) {
    return Ban(
      id: json["id"],
      userId: json["userId"],
      reason: json["reason"],
      issuedBy: json["issuedBy"],
      issuedAt: DateTime.parse(json["issuedAt"]),
      expiresAt: json["expiresAt"] == null
          ? null
          : DateTime.parse(json["expiresAt"]),
      isActive: json["isActive"],
    );
  }
}

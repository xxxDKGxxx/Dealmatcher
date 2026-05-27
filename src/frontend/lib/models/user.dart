class User {
  const User({
    required this.id,
    required this.email,
    required this.name,
    required this.surname,
    required this.status,
    required this.createdAt,
  });

  final int id;
  final String email;
  final String name;
  final String surname;
  final UserStatus status;
  final DateTime createdAt;

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] as int,
      email: json['email'] as String,
      name: json['name'] as String,
      surname: json['surname'] as String,
      status: UserStatus.fromString(json['status'] as String),
      createdAt: DateTime.parse(json['createdAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'email': email,
      'name': name,
      'surname': surname,
      'status': status.name,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}

enum UserStatus {
  admin,
  active,
  inactive,
  banned;

  static UserStatus fromString(String s) {
    return UserStatus.values.firstWhere(
      (e) => e.toString() == 'UserStatus.${s.toLowerCase()}',
    );
  }
}

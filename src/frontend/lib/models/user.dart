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
}

enum UserStatus {
  active,
  inactive,
  banned;

  static UserStatus fromString(String s) {
    return UserStatus.values.firstWhere(
      (e) => e.toString() == 'UserStatus.${s.toLowerCase()}',
    );
  }
}

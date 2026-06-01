import 'package:frontend/models/user.dart';

class AllUsers {
  const AllUsers({
    required this.items,
    required this.total,
    required this.page,
    required this.pages,
  });

  final List<User> items;
  final int total;
  final int page;
  final int pages;

  factory AllUsers.fromJson(Map<String, dynamic> json) {
    final List<User> items = [];
    if (json['items'] != null) {
      for (var item in json['items']) {
        items.add(User.fromJson(item));
      }
    }

    return AllUsers(
      items: items,
      total: json['total'],
      page: json['page'],
      pages: json['pages'],
    );
  }
}

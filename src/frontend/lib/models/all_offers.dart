import 'package:frontend/models/offer.dart';

class AllOffers {
  const AllOffers({
    required this.items,
    required this.total,
    required this.page,
    required this.pages,
  });

  final List<Offer> items;
  final int total;
  final int page;
  final int pages;

  factory AllOffers.fromJson(Map<String, dynamic> json) {
    final List<Offer> items = [];
    if (json['items'] != null) {
      for (var item in json['items']) {
        items.add(Offer.fromJson(item));
      }
    }

    return AllOffers(
      items: items,
      total: json['total'],
      page: json['page'],
      pages: json['pages'],
    );
  }
}

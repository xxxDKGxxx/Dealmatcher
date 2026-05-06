import 'package:frontend/models/offer.dart';

final class CartItem {
  const CartItem({
    required this.id,
    required this.offer,
    required this.quantity,
    required this.addedAt,
  });

  final int id;
  final Offer offer;
  final int quantity;
  final DateTime addedAt;

  factory CartItem.fromJson(Map<String, dynamic> json) {
    return CartItem(
      id: json['id'],
      offer: Offer.fromJson(json['offer']),
      quantity: json['quantity'],
      addedAt: DateTime.parse(json['addedAt']),
    );
  }
}

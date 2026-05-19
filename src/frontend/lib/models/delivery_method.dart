final class DeliveryMethod {
  final String id;
  final String name;
  final String description;
  final double price;
  final int estimatedDays;

  DeliveryMethod({
    required this.id,
    required this.name,
    required this.description,
    required this.price,
    required this.estimatedDays,
  });

  factory DeliveryMethod.fromJson(Map<String, dynamic> json) {
    return DeliveryMethod(
      id: json['id'] as String,
      name: json['name'] as String,
      description: json['description'] as String,
      price: (json['price'] as num).toDouble(),
      estimatedDays: json['estimatedDays'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'price': price,
      'estimatedDays': estimatedDays,
    };
  }
}

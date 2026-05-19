final class PaymentMethod {
  final String id;
  final String name;
  final String provider;
  final String icon;

  PaymentMethod({
    required this.id,
    required this.name,
    required this.provider,
    required this.icon,
  });

  factory PaymentMethod.fromJson(Map<String, dynamic> json) {
    return PaymentMethod(
      id: json['id'] as String,
      name: json['name'] as String,
      provider: json['provider'] as String,
      icon: json['icon'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'name': name, 'provider': provider, 'icon': icon};
  }
}

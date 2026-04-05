final class Property {
  final int id;
  final String name;
  final PropertyType type;
  final List<String> options;

  Property({
    required this.id,
    required this.name,
    required this.type,
    required this.options
  });
}

enum PropertyType {
  NUMBER,
  BOOLEAN,
  SELECT
}
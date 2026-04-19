final class PropertyDefinition {
  final int id;
  final String name;
  final PropertyType type;
  final List<String> options;

  PropertyDefinition({
    required this.id,
    required this.name,
    required this.type,
    required this.options,
  });
}

enum PropertyType { number, boolean, select, text }

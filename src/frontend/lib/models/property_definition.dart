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

  factory PropertyDefinition.fromJson(Map<String, dynamic> json) {
    final List<String> parsedOptions = [];
    if (json['options'] != null) {
      for (var option in json['options']) {
        parsedOptions.add(option.toString());
      }
    }

    return PropertyDefinition(
      id: json['id'] as int,
      name: json['name'] as String,
      type: PropertyType.fromString(json['type'] as String),
      options: parsedOptions,
    );
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'name': name, 'type': type.name, 'options': options};
  }
}

enum PropertyType {
  number,
  boolean,
  select,
  text;

  static PropertyType fromString(String s) {
    return PropertyType.values.firstWhere(
      (e) => e.toString() == 'PropertyType.${s.toLowerCase()}',
    );
  }
}

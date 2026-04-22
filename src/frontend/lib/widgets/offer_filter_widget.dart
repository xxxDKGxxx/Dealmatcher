import 'package:flutter/material.dart';

import '../models/category.dart';
import '../models/property_definition.dart';
import 'form_fields.dart';

class OfferFilterWidget extends StatefulWidget {
  final void Function(Map<String, dynamic> filters)? onFilterChanged;

  const OfferFilterWidget({super.key, this.onFilterChanged});

  @override
  State<OfferFilterWidget> createState() => _OfferFilterWidgetState();
}

class _OfferFilterWidgetState extends State<OfferFilterWidget> {
  late Future<List<Category>> _categoriesFuture;
  Category? _selectedCategory;
  Future<List<PropertyDefinition>>? _propertiesFuture;

  // Global filters
  String _phrase = "";
  double? _priceMin;
  double? _priceMax;
  final List<String> _tags = [];
  final TextEditingController _tagController = TextEditingController();

  // Category specific property filters
  final Map<int, List<String>> _properties = {};

  @override
  void initState() {
    super.initState();
    _categoriesFuture = _fetchCategories();
  }

  void _updateFilters() {
    if (widget.onFilterChanged != null) {
      widget.onFilterChanged!({
        'phrase': _phrase,
        'priceMin': _priceMin,
        'priceMax': _priceMax,
        'tags': _tags,
        'categoryId': _selectedCategory?.id,
        'properties': _properties,
      });
    }
  }

  void _addTag() {
    final tag = _tagController.text.trim();
    if (tag.isNotEmpty && !_tags.contains(tag)) {
      setState(() {
        _tags.add(tag);
        _tagController.clear();
        _updateFilters();
      });
    }
  }

  void _removeTag(String tag) {
    setState(() {
      _tags.remove(tag);
      _updateFilters();
    });
  }

  Widget _buildFieldContainer(Widget child) {
    return SizedBox(height: 70, child: child);
  }

  Future<List<Category>> _fetchCategories() async {
    await Future.delayed(const Duration(seconds: 1));
    return [
      Category(
        id: 0,
        name: "Computers",
        description: "PC, Laptops and Notebooks",
      ),
      Category(
        id: 1,
        name: "Apartements",
        description: "Apartements for rent or for sale",
      ),
    ];
  }

  Future<List<PropertyDefinition>> _fetchProperties(int categoryId) async {
    await Future.delayed(const Duration(milliseconds: 500));
    if (categoryId == 0) {
      return [
        PropertyDefinition(
          id: 0,
          name: 'Model',
          type: PropertyType.text,
          options: [],
        ),
        PropertyDefinition(
          id: 1,
          name: "RAM (GB)",
          type: PropertyType.numeric,
          options: [],
        ),
        PropertyDefinition(
          id: 2,
          name: "Storage (GB)",
          type: PropertyType.numeric,
          options: [],
        ),
        PropertyDefinition(
          id: 3,
          name: "OS",
          type: PropertyType.select,
          options: ["Windows", "Linux", "MacOS"],
        ),
        PropertyDefinition(
          id: 4,
          name: "Is New",
          type: PropertyType.boolean,
          options: [],
        ),
      ];
    } else if (categoryId == 1) {
      return [
        PropertyDefinition(
          id: 5,
          name: "Rooms",
          type: PropertyType.numeric,
          options: [],
        ),
        PropertyDefinition(
          id: 6,
          name: "Floor",
          type: PropertyType.numeric,
          options: [],
        ),
        PropertyDefinition(
          id: 7,
          name: "Has Balcony",
          type: PropertyType.boolean,
          options: [],
        ),
        PropertyDefinition(
          id: 8,
          name: "Heating",
          type: PropertyType.select,
          options: ["Gas", "Electric", "Central"],
        ),
      ];
    }
    return [];
  }

  Widget _buildPropertyFilter(PropertyDefinition prop) {
    switch (prop.type) {
      case PropertyType.numeric:
        _properties.putIfAbsent(prop.id, () => ["", ""]);
        return Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: _buildFieldContainer(
                numberFormField(
                  text: "${prop.name} (Min)",
                  onChanged: (val) {
                    _properties[prop.id]![0] = val;
                    _updateFilters();
                  },
                  validator: (val) {
                    if (val != null &&
                        val.trim().isNotEmpty &&
                        double.tryParse(val) == null) {
                      return "Invalid number";
                    }
                    return null;
                  },
                ),
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: _buildFieldContainer(
                numberFormField(
                  text: "${prop.name} (Max)",
                  onChanged: (val) {
                    _properties[prop.id]![1] = val;
                    _updateFilters();
                  },
                  validator: (val) {
                    if (val != null &&
                        val.trim().isNotEmpty &&
                        double.tryParse(val) == null) {
                      return "Invalid number";
                    }
                    return null;
                  },
                ),
              ),
            ),
          ],
        );
      case PropertyType.boolean:
        return CheckboxListTile(
          title: Text(prop.name),
          value:
              _properties[prop.id]?.isNotEmpty == true &&
              _properties[prop.id]![0] == "true",
          onChanged: (val) {
            setState(() {
              _properties[prop.id] = [(val ?? false).toString()];
              _updateFilters();
            });
          },
          controlAffinity: ListTileControlAffinity.leading,
          contentPadding: EdgeInsets.zero,
        );
      case PropertyType.text:
        return _buildFieldContainer(
          TextFormField(
            onChanged: (val) {
              _properties[prop.id] = [val];
              _updateFilters();
            },
            decoration: InputDecoration(
              labelText: prop.name,
              border: const OutlineInputBorder(),
            ),
          ),
        );
      case PropertyType.select:
        final selectedOptions = _properties[prop.id] ?? [];
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              prop.name,
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8.0,
              children: prop.options.map((opt) {
                final isSelected = selectedOptions.contains(opt);
                return FilterChip(
                  label: Text(opt),
                  selected: isSelected,
                  onSelected: (selected) {
                    setState(() {
                      if (selected) {
                        _properties.putIfAbsent(prop.id, () => []).add(opt);
                      } else {
                        _properties[prop.id]?.remove(opt);
                      }
                      _updateFilters();
                    });
                  },
                );
              }).toList(),
            ),
          ],
        );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16.0),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        borderRadius: BorderRadius.circular(8),
        boxShadow: const [
          BoxShadow(color: Colors.black12, blurRadius: 4, offset: Offset(0, 2)),
        ],
      ),
      child: Form(
        autovalidateMode: AutovalidateMode.onUserInteraction,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisSize: MainAxisSize.min,
          children: [
            const Text(
              "Filter Offers",
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            _buildFieldContainer(
              TextFormField(
                onChanged: (val) {
                  _phrase = val;
                  _updateFilters();
                },
                decoration: const InputDecoration(
                  labelText: "Search Phrase",
                  border: OutlineInputBorder(),
                ),
              ),
            ),
            const SizedBox(height: 8),
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: _buildFieldContainer(
                    numberFormField(
                      text: "Price (Min)",
                      onChanged: (val) {
                        _priceMin = double.tryParse(val);
                        _updateFilters();
                      },
                      validator: (val) {
                        if (val != null &&
                            val.trim().isNotEmpty &&
                            double.tryParse(val) == null) {
                          return "Invalid number";
                        }
                        return null;
                      },
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: _buildFieldContainer(
                    numberFormField(
                      text: "Price (Max)",
                      onChanged: (val) {
                        _priceMax = double.tryParse(val);
                        _updateFilters();
                      },
                      validator: (val) {
                        if (val != null &&
                            val.trim().isNotEmpty &&
                            double.tryParse(val) == null) {
                          return "Invalid number";
                        }
                        return null;
                      },
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            const Text("Tags", style: TextStyle(fontWeight: FontWeight.bold)),
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _tagController,
                    decoration: const InputDecoration(labelText: "Add Tag"),
                  ),
                ),
                IconButton(icon: const Icon(Icons.add), onPressed: _addTag),
              ],
            ),
            Wrap(
              spacing: 8,
              children: _tags
                  .map(
                    (tag) => Chip(
                      label: Text(tag),
                      onDeleted: () => _removeTag(tag),
                    ),
                  )
                  .toList(),
            ),
            const SizedBox(height: 16),
            FutureBuilder<List<Category>>(
              future: _categoriesFuture,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator());
                }
                if (snapshot.hasError) {
                  return Text('Error loading categories: ${snapshot.error}');
                }
                if (snapshot.hasData) {
                  final categories = snapshot.data!;
                  return dropdownFormField<Category>(
                    text: 'Filter by category',
                    value: _selectedCategory,
                    items: [
                      const DropdownMenuItem<Category>(
                        value: null,
                        child: Text("All Categories"),
                      ),
                      ...categories.map(
                        (cat) => DropdownMenuItem<Category>(
                          value: cat,
                          child: Text(cat.name),
                        ),
                      ),
                    ],
                    onChanged: (newValue) {
                      setState(() {
                        _selectedCategory = newValue;
                        _properties.clear();
                        if (newValue != null) {
                          _propertiesFuture = _fetchProperties(newValue.id);
                        } else {
                          _propertiesFuture = null;
                        }
                        _updateFilters();
                      });
                    },
                    validator: (val) => null, // Filtering is optional
                  );
                }
                return const SizedBox();
              },
            ),
            const SizedBox(height: 16),
            if (_propertiesFuture != null)
              FutureBuilder<List<PropertyDefinition>>(
                future: _propertiesFuture,
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting) {
                    return const Center(child: CircularProgressIndicator());
                  }
                  if (snapshot.hasData && snapshot.data!.isNotEmpty) {
                    return Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Divider(),
                        const SizedBox(height: 8),
                        const Text(
                          "Category Properties",
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 16),
                        ...snapshot.data!.map(
                          (prop) => Padding(
                            padding: const EdgeInsets.only(bottom: 16.0),
                            child: _buildPropertyFilter(prop),
                          ),
                        ),
                      ],
                    );
                  }
                  return const SizedBox();
                },
              ),
          ],
        ),
      ),
    );
  }
}

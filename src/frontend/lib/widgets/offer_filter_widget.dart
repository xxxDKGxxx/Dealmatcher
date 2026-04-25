import 'package:flutter/material.dart';
import 'package:frontend/api/api_categories.dart';

import '../models/category.dart';
import '../models/property_definition.dart';
import 'form_fields.dart';

class OfferFilterWidget extends StatefulWidget {
  final Map<String, dynamic>? initialFilters;
  final void Function(Map<String, dynamic> filters)? onFilterChanged;

  const OfferFilterWidget({
    super.key,
    this.initialFilters,
    this.onFilterChanged,
  });

  @override
  State<OfferFilterWidget> createState() => _OfferFilterWidgetState();
}

class _OfferFilterWidgetState extends State<OfferFilterWidget> {
  late Future<List<Category>> _categoriesFuture;
  Category? _selectedCategory;
  Future<List<PropertyDefinition>>? _propertiesFuture;

  // Global filters
  late String _phrase;
  late double? _priceMin;
  late double? _priceMax;
  late final List<String> _tags;
  late final TextEditingController _tagController;

  // Category specific property filters
  late final Map<String, List<String>> _properties;

  @override
  void initState() {
    super.initState();
    final filters = widget.initialFilters ?? {};
    _phrase = filters['searchPhrase'] ?? "";
    _priceMin = filters['minPrice'];
    _priceMax = filters['maxPrice'];
    _tags = List<String>.from(filters['tags'] ?? []);
    _tagController = TextEditingController();
    _properties = Map<String, List<String>>.from(filters['properties'] ?? {});

    _categoriesFuture = _fetchCategories().then((categories) {
      if (filters['categoryId'] != null) {
        try {
          final catId = filters['categoryId'];
          final selected = categories.firstWhere((c) => c.id == catId);
          setState(() {
            _selectedCategory = selected;
            _propertiesFuture = _fetchProperties(selected.name);
          });
        } catch (_) {}
      }
      return categories;
    });
  }

  void _updateFilters() {
    if (widget.onFilterChanged != null) {
      widget.onFilterChanged!({
        'searchPhrase': _phrase,
        'minPrice': _priceMin,
        'maxPrice': _priceMax,
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
    return ApiCategories().getCategories();
  }

  Future<List<PropertyDefinition>> _fetchProperties(String categoryName) async {
    return ApiCategories().getPropertyDefinitions(categoryName);
  }

  Widget _buildPropertyFilter(PropertyDefinition prop) {
    switch (prop.type) {
      case PropertyType.numeric:
        final propValues = _properties[prop.id.toString()];
        final minVal = (propValues != null && propValues.isNotEmpty)
            ? propValues[0]
            : "";
        final maxVal = (propValues != null && propValues.length > 1)
            ? propValues[1]
            : "";
        return Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: _buildFieldContainer(
                numberFormField(
                  text: "${prop.name} (Min)",
                  initialValue: minVal,
                  onChanged: (val) {
                    _properties.putIfAbsent(prop.id.toString(), () => ["", ""]);
                    _properties[prop.id.toString()]![0] = val;

                    if (_properties[prop.id.toString()]![0].isEmpty &&
                        _properties[prop.id.toString()]![1].isEmpty) {
                      _properties.remove(prop.id.toString());
                    }
                    _updateFilters();
                  },
                  validator: (val) {
                    if (val != null &&
                        val.trim().isNotEmpty &&
                        double.tryParse(val) == null) {
                      return "Invalid number";
                    }
                    if (_properties[prop.id.toString()] != null &&
                        _properties[prop.id.toString()]!.length > 1 &&
                        _properties[prop.id.toString()]![1].isNotEmpty &&
                        (val == null || val.isEmpty)) {
                      return "Required when providing range";
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
                  initialValue: maxVal,
                  onChanged: (val) {
                    _properties.putIfAbsent(prop.id.toString(), () => ["", ""]);
                    _properties[prop.id.toString()]![1] = val;

                    if (_properties[prop.id.toString()]![0].isEmpty &&
                        _properties[prop.id.toString()]![1].isEmpty) {
                      _properties.remove(prop.id.toString());
                    }
                    _updateFilters();
                  },
                  validator: (val) {
                    if (val != null &&
                        val.trim().isNotEmpty &&
                        double.tryParse(val) == null) {
                      return "Invalid number";
                    }
                    if (_properties[prop.id.toString()] != null &&
                        _properties[prop.id.toString()]!.isNotEmpty &&
                        _properties[prop.id.toString()]![0].isNotEmpty &&
                        (val == null || val.isEmpty)) {
                      return "Required when providing range";
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
              _properties[prop.id.toString()]?.isNotEmpty == true &&
              _properties[prop.id.toString()]![0] == "true",
          onChanged: (val) {
            setState(() {
              _properties[prop.id.toString()] = [(val ?? false).toString()];
              _updateFilters();
            });
          },
          controlAffinity: ListTileControlAffinity.leading,
          contentPadding: EdgeInsets.zero,
        );
      case PropertyType.text:
        final initVal = _properties[prop.id.toString()]?.isNotEmpty == true
            ? _properties[prop.id.toString()]![0]
            : "";
        return _buildFieldContainer(
          TextFormField(
            initialValue: initVal,
            onChanged: (val) {
              _properties[prop.id.toString()] = [val];
              _updateFilters();
            },
            decoration: InputDecoration(
              labelText: prop.name,
              border: const OutlineInputBorder(),
            ),
          ),
        );
      case PropertyType.select:
        final selectedOptions = _properties[prop.id.toString()] ?? [];
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
                        _properties
                            .putIfAbsent(prop.id.toString(), () => [])
                            .add(opt);
                      } else {
                        _properties[prop.id.toString()]?.remove(opt);
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
                initialValue: _phrase,
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
                      initialValue: _priceMin?.toString(),
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
                      initialValue: _priceMax?.toString(),
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
                    itemLabelBuilder: (cat) => cat.name,
                    onChanged: (newValue) {
                      setState(() {
                        _selectedCategory = newValue;
                        _properties.clear();
                        if (newValue != null) {
                          _propertiesFuture = _fetchProperties(newValue.name);
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

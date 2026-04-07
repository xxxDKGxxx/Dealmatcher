import 'dart:io';

import 'package:flutter/material.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/form_fields.dart';
import 'package:frontend/widgets/property_field.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

import '../models/category.dart';
import '../models/property.dart';

class CreateOfferPage extends StatefulWidget {
  const CreateOfferPage({super.key});

  @override
  State<CreateOfferPage> createState() => _CreateOfferPageState();
}

class _CreateOfferPageState extends State<CreateOfferPage> {
  final _formKey = GlobalKey<FormState>();
  final ImagePicker _picker = ImagePicker();

  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _descriptionController = TextEditingController();
  final TextEditingController _priceController = TextEditingController();
  final TextEditingController _availabilityController = TextEditingController();

  late Future<List<Category>> _categoriesFuture;
  Category? _selectedCategory;

  final TextEditingController _tagController = TextEditingController();
  final List<String> _tags = [];

  Future<List<Property>>? _propertiesFuture;
  final Map<String, dynamic> _properties = {};

  final List<XFile> _images = [];

  Future<void> _pickImage() async {
    final List<XFile> selectedImages = await _picker.pickMultiImage();
    if (selectedImages.isNotEmpty) {
      setState(() {
        _images.addAll(selectedImages);
      });
    }
  }

  void _removeImage(int index) {
    setState(() {
      _images.removeAt(index);
    });
  }

  void _addTag() {
    final tag = _tagController.text.trim();
    if (tag.isNotEmpty && !_tags.contains(tag)) {
      setState(() {
        _tags.add(tag);
        _tagController.clear();
      });
    }
  }

  void _removeTag(String tag) {
    setState(() {
      _tags.remove(tag);
    });
  }

  void _submit() {
    if (_formKey.currentState!.validate()) {
      // TODO Replace this with retrofit when doing api integration
      final data = {
        "title": _titleController.text,
        "description": _descriptionController.text,
        "price": double.tryParse(_priceController.text) ?? 0,
        "images": _images
            .map((e) => e.path)
            .toList(), // In future this would be base64 or upload paths
        "categoryId": _selectedCategory?.id,
        "tags": _tags,
        "properties": _properties,
        "availability": int.tryParse(_availabilityController.text) ?? 1,
      };

      debugPrint("offer data: $data");

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('offer form is valid.')));
    }
  }

  Future<List<Category>> _fetchCategories() async {
    await Future.delayed(const Duration(seconds: 2));
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

  Future<List<Property>> _fetchProperties(int categoryId) async {
    await Future.delayed(const Duration(seconds: 1));
    if (categoryId == 0) {
      // Computers
      return [
        Property(id: 0, name: 'Model', type: PropertyType.text, options: []),
        Property(
          id: 1,
          name: "RAM (GB)",
          type: PropertyType.number,
          options: [],
        ),
        Property(
          id: 2,
          name: "Storage (GB)",
          type: PropertyType.number,
          options: [],
        ),
        Property(
          id: 3,
          name: "OS",
          type: PropertyType.select,
          options: ["Windows", "Linux", "MacOS"],
        ),
        Property(
          id: 4,
          name: "Is New",
          type: PropertyType.boolean,
          options: [],
        ),
      ];
    } else if (categoryId == 1) {
      // Apartments
      return [
        Property(id: 5, name: "Rooms", type: PropertyType.number, options: []),
        Property(id: 6, name: "Floor", type: PropertyType.number, options: []),
        Property(
          id: 7,
          name: "Has Balcony",
          type: PropertyType.boolean,
          options: [],
        ),
        Property(
          id: 8,
          name: "Heating",
          type: PropertyType.select,
          options: ["Gas", "Electric", "Central"],
        ),
      ];
    }
    return [];
  }

  @override
  void initState() {
    super.initState();
    _categoriesFuture = _fetchCategories();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 800),
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Form(
              key: _formKey,
              child: CustomScrollView(
                slivers: [
                  SliverList.list(
                    children: [
                      const Text(
                        "Add new offer",
                        style: TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 20),
                      nonEmptyTextFormField(
                        controller: _titleController,
                        text: "Title",
                      ),
                      const SizedBox(height: 16),
                      nonEmptyTextFormField(
                        controller: _descriptionController,
                        text: "Description",
                        maxLines: 4,
                      ),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            child: numberFormField(
                              controller: _priceController,
                              text: "Price",
                            ),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            child: numberFormField(
                              controller: _availabilityController,
                              text: "Availability",
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      FutureBuilder<List<Category>>(
                        future: _categoriesFuture,
                        builder: (context, snapshot) {
                          if (snapshot.connectionState ==
                              ConnectionState.waiting) {
                            return const Center(
                              child: CircularProgressIndicator(),
                            );
                          }
                          if (snapshot.hasError) {
                            return Text(
                              'Error loading categories: ${snapshot.error}',
                            );
                          }
                          if (snapshot.hasData) {
                            final categories = snapshot.data!;

                            return dropdownFormField<Category>(
                              text: 'Choose category',
                              value: _selectedCategory,
                              items: categories
                                  .map(
                                    (cat) => DropdownMenuItem<Category>(
                                      value: cat,
                                      child: Column(
                                        crossAxisAlignment:
                                            CrossAxisAlignment.start,
                                        mainAxisSize: MainAxisSize.min,
                                        children: [
                                          Text(
                                            cat.name,
                                            style: const TextStyle(
                                              fontWeight: FontWeight.bold,
                                              fontSize: 16,
                                            ),
                                          ),
                                          const SizedBox(height: 16),
                                          Text(
                                            cat.description,
                                            style: const TextStyle(
                                              color: Colors.grey,
                                              fontSize: 13,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  )
                                  .toList(),
                              onChanged: (newValue) {
                                setState(() {
                                  _selectedCategory = newValue;
                                  _properties.clear();
                                  if (newValue != null) {
                                    _propertiesFuture = _fetchProperties(
                                      newValue.id,
                                    );
                                  } else {
                                    _propertiesFuture = null;
                                  }
                                });
                              },
                              validator: (value) {
                                if (value == null) {
                                  return 'Category is required';
                                }

                                return null;
                              },
                            );
                          }
                          return const Text("No categories available");
                        },
                      ),
                      const SizedBox(height: 32),

                      // Images Section
                      const Text(
                        "Images",
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 8),
                      SizedBox(
                        height: 120,
                        child: ListView.builder(
                          scrollDirection: Axis.horizontal,
                          itemCount: _images.length + 1,
                          itemBuilder: (context, index) {
                            if (index == _images.length) {
                              return Padding(
                                padding: const EdgeInsets.all(8.0),
                                child: InkWell(
                                  onTap: _pickImage,
                                  child: Container(
                                    width: 100,
                                    decoration: BoxDecoration(
                                      border: Border.all(color: Colors.grey),
                                      borderRadius: BorderRadius.circular(8),
                                    ),
                                    child: const Icon(
                                      Icons.add_a_photo,
                                      size: 40,
                                    ),
                                  ),
                                ),
                              );
                            }
                            return Stack(
                              children: [
                                Padding(
                                  padding: const EdgeInsets.all(8.0),
                                  child: ClipRRect(
                                    borderRadius: BorderRadius.circular(8),
                                    child: kIsWeb
                                        ? Image.network(
                                            _images[index].path,
                                            width: 100,
                                            height: 100,
                                            fit: BoxFit.cover,
                                          )
                                        : Image.file(
                                            File(_images[index].path),
                                            width: 100,
                                            height: 100,
                                            fit: BoxFit.cover,
                                          ),
                                  ),
                                ),
                                Positioned(
                                  right: 0,
                                  top: 0,
                                  child: IconButton(
                                    icon: const Icon(
                                      Icons.remove_circle,
                                      color: Colors.red,
                                    ),
                                    onPressed: () => _removeImage(index),
                                  ),
                                ),
                              ],
                            );
                          },
                        ),
                      ),
                      const SizedBox(height: 32),

                      // Tags Section
                      const Text(
                        "Tags",
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Row(
                        children: [
                          Expanded(
                            child: TextField(
                              controller: _tagController,
                              decoration: const InputDecoration(
                                labelText: "Add Tag",
                              ),
                            ),
                          ),
                          IconButton(
                            icon: const Icon(Icons.add),
                            onPressed: _addTag,
                          ),
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
                      const SizedBox(height: 32),

                      // Properties Section
                      FutureBuilder<List<Property>>(
                        future: _propertiesFuture,
                        builder: (context, snapshot) {
                          if (_selectedCategory == null) {
                            return const Center(
                              child: Text(
                                "Please select a category to see its properties",
                              ),
                            );
                          }
                          if (snapshot.connectionState ==
                              ConnectionState.waiting) {
                            return const Center(
                              child: CircularProgressIndicator(),
                            );
                          }
                          if (snapshot.hasError) {
                            return Text(
                              'Error loading properties: ${snapshot.error}',
                            );
                          }
                          if (!snapshot.hasData || snapshot.data!.isEmpty) {
                            return const Text(
                              "No specific properties for this category",
                            );
                          }

                          final properties = snapshot.data!;

                          return Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              const Text(
                                "Category Specific Properties",
                                style: TextStyle(
                                  fontSize: 18,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 16),
                              ...properties.map(
                                (prop) => PropertyField(
                                  property: prop,
                                  value: _properties[prop.name],
                                  onChanged: (newValue) {
                                    setState(() {
                                      _properties[prop.name] = newValue;
                                    });
                                  },
                                ),
                              ),
                            ],
                          );
                        },
                      ),
                      const SizedBox(height: 48),

                      ElevatedButton(
                        onPressed: _submit,
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                        ),
                        child: const Text(
                          "Create Offer",
                          style: TextStyle(fontSize: 18),
                        ),
                      ),
                      const SizedBox(height: 64),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

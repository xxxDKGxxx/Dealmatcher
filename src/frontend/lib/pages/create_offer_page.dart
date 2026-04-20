import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_cache_manager/flutter_cache_manager.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/form_fields.dart';
import 'package:frontend/widgets/property_field.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';
import 'package:flutter/foundation.dart' show kIsWeb;

import '../models/category.dart';
import '../models/property_definition.dart';

class CreateOfferPage extends StatefulWidget {
  const CreateOfferPage({super.key, this.offerId});

  final int? offerId;

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

  Future<List<PropertyDefinition>>? _propertiesFuture;
  final Map<String, dynamic> _properties = {};

  final List<XFile> _images = [];

  late Future<Offer?>? _offerFuture;
  late Offer? offer;

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
      final isUpdate = widget.offerId != null;
      final data = {
        "id": widget.offerId,
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

      debugPrint(isUpdate ? "updated data: $data" : "created offer data: $data");

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(isUpdate ? 'Updated offer' : 'Created new offer')));
    }
  }

  Future<Offer?> _fetchOffer() async {
    await Future.delayed(const Duration(seconds: 1, milliseconds: 500));
    if(widget.offerId == null) {
      return null;
    }

    offer = Offer(
        id: widget.offerId!,
        title: "Polish Cow",
        description: "Tylko jedno w głowie mam",
        price: 133000,
        images: [
          'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.suwalki24.pl%2F_uploads%2F2020%2FGrudzien%2FDrobne%2Fkrowy_pasace_sie.jpg&f=1&nofb=1&ipt=7ac6e204693a3b2b87cc07b9c800f7de5bf5e627e31f6e646dc28c4b8a9f4f93',
          'https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi.ytimg.com%2Fvi%2FrfcliTe0qAs%2Fmaxresdefault.jpg&f=1&nofb=1&ipt=e61737e24b54abdd3cdb348ba66d42a572b95bb45202791838496c6c8d393320',
        ],
        seller: Seller(id: 0, name: 'Zenon'),
        category: (await _fetchCategories())[1], //Category(id: 1, name: 'Animal', description: 'living creatures to eat in future'),
        tags: ['animal', 'cow', 'yummy', 'i', 'hate', 'io'],
        properties: {
          0: '12',
          1: '59',
          2: 'true',
          3: 'Central',
        },
        availability: (widget.offerId! + 21) * 37,
        status: OfferStatus.active,
        createdAt: DateTime.now().subtract(Duration(days: 5)),
        updatedAt: DateTime.now().subtract(Duration(hours: 21)),
    );

    _titleController.text = offer!.title;
    _descriptionController.text = offer!.description;
    _priceController.text = offer!.price.toString();
    _availabilityController.text = offer!.availability.toString();
    //_tagController.text = offer!.tags.join(', ');
    _selectedCategory = offer!.category;
    _propertiesFuture = _fetchProperties(_selectedCategory!.id);
    for(var t in offer!.tags) {
      _tags.add(t);
    }
    for(var i in offer!.images){
      var file = await DefaultCacheManager().getSingleFile(i);
      _images.add(XFile(file.path));
    }
    final properties = await _fetchProperties(_selectedCategory!.id);
    for (var i = 0; i < properties.length; i++) {
      dynamic value;
      switch(properties[i].type) {
        case PropertyType.number:
          value = int.tryParse(offer!.properties[i] ?? '');
          break;
        case PropertyType.boolean:
          value = bool.tryParse(offer!.properties[i] ?? '');
          break;
        case PropertyType.text:
          value = offer!.properties[i];
          break;
        case PropertyType.select:
          value = offer!.properties[i];
          break;
      }
      _properties[properties[i].name] = value;
    }

    return offer;
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

  Future<List<PropertyDefinition>> _fetchProperties(int categoryId) async {
    await Future.delayed(const Duration(seconds: 1));
    if (categoryId == 0) {
      // Computers
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
          type: PropertyType.number,
          options: [],
        ),
        PropertyDefinition(
          id: 2,
          name: "Storage (GB)",
          type: PropertyType.number,
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
      // Apartments
      return [
        PropertyDefinition(
          id: 5,
          name: "Rooms",
          type: PropertyType.number,
          options: [],
        ),
        PropertyDefinition(
          id: 6,
          name: "Floor",
          type: PropertyType.number,
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
  
  void _deleteOffer(int id) {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Successfully deleted this offer')),
    );
    context.pop();
  }

  @override
  void initState() {
    super.initState();
    _categoriesFuture = _fetchCategories();

    if(widget.offerId != null) {
      _offerFuture = _fetchOffer();
    }
    else {
      _offerFuture = null;
    }
  }

  @override
  Widget build(BuildContext context) {
    final isUpdated = widget.offerId != null;
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 800),
          child: Padding(
            padding: const EdgeInsets.all(16.0),
              child: FutureBuilder(
                future: _offerFuture,
                builder: (context, snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting) {
                    return const Center(child: CircularProgressIndicator());
                  } else if(snapshot.hasError) {
                    return Center(child: Text('Error loading offer: ${snapshot.error.toString()}'));
                  }
                  return Form(
                    key: _formKey,
                    child: CustomScrollView(
                      slivers: [
                        SliverList.list(
                          children: [
                            Text(
                              isUpdated ? "Update offer" : "Add new offer",
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
                                  final categoriesWithOfferId = categories.where((c) => c.id == _selectedCategory?.id);
                                  if(categoriesWithOfferId.isEmpty){
                                    _selectedCategory = null;
                                  } else {
                                    _selectedCategory = categoriesWithOfferId.first;
                                  }

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
                                    itemLabelBuilder: (cat) => cat.name,
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
                            FutureBuilder<List<PropertyDefinition>>(
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

                                // if (offer != null) {
                                //   for (var i = 0; i < properties.length; i++) {
                                //     properties[i] = offer!.properties
                                //   }
                                // }

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
                              child: Text(
                                isUpdated ? "Update Offer" : "Create Offer",
                                style: TextStyle(fontSize: 18),
                              ),
                            ),
                            if (isUpdated) ...[
                              const SizedBox(height: 16),
                              ElevatedButton(
                                onPressed: () => _deleteOffer(offer!.id),
                                style: ElevatedButton.styleFrom(
                                  padding: const EdgeInsets.symmetric(vertical: 16),
                                  foregroundColor: Colors.pink.shade900,
                                  backgroundColor: Colors.red.shade400,
                                ),
                                child: Text(
                                    'Delete Offer',
                                  style: TextStyle(fontSize: 18),
                                ),
                              ),
                            ],
                            const SizedBox(height: 64),
                          ],
                        ),
                      ],
                    ),
                  );
                }
              ),
          ),
        ),
      ),
    );
  }
}

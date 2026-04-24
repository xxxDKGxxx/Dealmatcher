import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_cache_manager/flutter_cache_manager.dart';
import 'package:frontend/api/api_categories.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/api/models/offer_create_request.dart';
import 'package:frontend/api/models/offer_update_request.dart';
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
  const CreateOfferPage({
    super.key,
    this.offerId,
    this.fetchCategories,
    this.fetchProperties,
    this.createOffer,
    this.updateOffer,
  });

  final int? offerId;
  final Future<List<Category>> Function()? fetchCategories;
  final Future<List<PropertyDefinition>> Function(String)? fetchProperties;
  final Future<void> Function(OfferCreateRequest)? createOffer;
  final Future<void> Function(Map<String, dynamic>)? updateOffer;

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
  final Map<String, String> _properties = {};

  final List<XFile> _images = [];

  late Future<Offer?>? _offerFuture;
  late Offer? offer;

  bool _isTitleEditing = false;
  bool _isDescriptionEditing = false;
  bool _isPriceEditing = false;
  bool _isAvailabilityEditing = false;
  bool _isTagsEditing = false;
  bool _isPropertiesEditing = false;
  bool _imagesChanged = false;

  Widget? _buildEditIcon(
    bool isUpdate,
    bool isEditing,
    VoidCallback onPressed,
  ) {
    if (!isUpdate) return null;
    return IconButton(
      icon: Icon(isEditing ? Icons.close : Icons.edit),
      onPressed: onPressed,
    );
  }

  Future<void> _pickImage() async {
    final List<XFile> selectedImages = await _picker.pickMultiImage();
    if (selectedImages.isNotEmpty) {
      setState(() {
        _images.addAll(selectedImages);
        _imagesChanged = true;
      });
    }
  }

  void _removeImage(int index) {
    setState(() {
      _images.removeAt(index);
      _imagesChanged = true;
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

  void _submit() async {
    if (_formKey.currentState!.validate()) {
      final isUpdate = widget.offerId != null;

      try {
        if (!isUpdate) {
          final request = OfferCreateRequest(
            title: _titleController.text,
            description: _descriptionController.text,
            price: double.tryParse(_priceController.text) ?? 0,
            images: _images,
            categoryId: _selectedCategory?.id ?? 0,
            tags: _tags,
            properties: _properties,
            availability: int.tryParse(_availabilityController.text) ?? 1,
          );

          if (widget.createOffer != null) {
            await widget.createOffer!(request);
          } else {
            await ApiOffers().createOffer(request);
          }

          if (mounted) {
            ScaffoldMessenger.of(
              context,
            ).showSnackBar(const SnackBar(content: Text('Created new offer')));
            context.go('/my-offers');
          }
        } else {
          final request = OfferUpdateRequest(
            title: _isTitleEditing ? _titleController.text : null,
            description: _isDescriptionEditing
                ? _descriptionController.text
                : null,
            price: _isPriceEditing
                ? (double.tryParse(_priceController.text) ?? 0)
                : null,
            availability: _isAvailabilityEditing
                ? (int.tryParse(_availabilityController.text) ?? 1)
                : null,
            images: _imagesChanged ? _images.map((e) => e.path).toList() : null,
            tags: _isTagsEditing ? _tags : null,
            properties: _isPropertiesEditing ? _properties : null,
          );

          if (widget.updateOffer != null) {
            await widget.updateOffer!(jsonDecode(request.toJson()));
          } else {
            await ApiOffers().updateOffer(widget.offerId!, request);
          }

          if (mounted) {
            ScaffoldMessenger.of(
              context,
            ).showSnackBar(const SnackBar(content: Text('Updated offer')));
          }

          context.go('/my-offers');
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(
            context,
          ).showSnackBar(SnackBar(content: Text('Error: ${e.toString()}')));
        }
      }
    }
  }

  Future<Offer?> _fetchOffer() async {
    if (widget.offerId == null) {
      return null;
    }

    offer = await ApiOffers().getOffer(widget.offerId!);

    _titleController.text = offer!.title;
    _descriptionController.text = offer!.description;
    _priceController.text = offer!.price.toString();
    _availabilityController.text = offer!.availability.toString();
    _selectedCategory = offer!.category;
    _propertiesFuture = _fetchProperties(_selectedCategory!.name);
    for (var t in offer!.tags) {
      _tags.add(t);
    }
    for (var i in offer!.images) {
      var file = await DefaultCacheManager().getSingleFile(i);
      _images.add(XFile(file.path));
    }
    final properties = await _fetchProperties(_selectedCategory!.name);
    for (var prop in properties) {
      if (offer!.properties.containsKey(prop.id)) {
        _properties[prop.id.toString()] = offer!.properties[prop.id]!;
      }
    }

    return offer;
  }

  Future<List<Category>> _fetchCategories() async {
    if (widget.fetchCategories != null) {
      return widget.fetchCategories!();
    }
    return ApiCategories().getCategories();
  }

  Future<List<PropertyDefinition>> _fetchProperties(String categoryName) async {
    if (widget.fetchProperties != null) {
      return widget.fetchProperties!(categoryName);
    }
    return ApiCategories().getPropertyDefinitions(categoryName);
  }

  Future _deleteOffer(int id) async {
    await ApiOffers().deleteOffer(id);

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Successfully deleted this offer')),
    );
    context.go('/my-offers');
  }

  @override
  void initState() {
    super.initState();
    _categoriesFuture = _fetchCategories();

    if (widget.offerId != null) {
      _offerFuture = _fetchOffer();
    } else {
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
                } else if (snapshot.hasError) {
                  return Center(
                    child: Text(
                      'Error loading offer: ${snapshot.error.toString()}',
                    ),
                  );
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
                            enabled: !isUpdated || _isTitleEditing,
                            suffixIcon: _buildEditIcon(
                              isUpdated,
                              _isTitleEditing,
                              () {
                                setState(
                                  () => _isTitleEditing = !_isTitleEditing,
                                );
                              },
                            ),
                          ),
                          const SizedBox(height: 16),
                          nonEmptyTextFormField(
                            controller: _descriptionController,
                            text: "Description",
                            maxLines: 4,
                            enabled: !isUpdated || _isDescriptionEditing,
                            suffixIcon: _buildEditIcon(
                              isUpdated,
                              _isDescriptionEditing,
                              () {
                                setState(
                                  () => _isDescriptionEditing =
                                      !_isDescriptionEditing,
                                );
                              },
                            ),
                          ),
                          const SizedBox(height: 16),
                          Row(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Expanded(
                                child: numberFormField(
                                  controller: _priceController,
                                  text: "Price",
                                  enabled: !isUpdated || _isPriceEditing,
                                  suffixIcon: _buildEditIcon(
                                    isUpdated,
                                    _isPriceEditing,
                                    () {
                                      setState(
                                        () =>
                                            _isPriceEditing = !_isPriceEditing,
                                      );
                                    },
                                  ),
                                ),
                              ),
                              const SizedBox(width: 16),
                              Expanded(
                                child: numberFormField(
                                  controller: _availabilityController,
                                  text: "Availability",
                                  enabled: !isUpdated || _isAvailabilityEditing,
                                  suffixIcon: _buildEditIcon(
                                    isUpdated,
                                    _isAvailabilityEditing,
                                    () {
                                      setState(
                                        () => _isAvailabilityEditing =
                                            !_isAvailabilityEditing,
                                      );
                                    },
                                  ),
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
                                final categoriesWithOfferId = categories.where(
                                  (c) => c.id == _selectedCategory?.id,
                                );
                                if (categoriesWithOfferId.isEmpty) {
                                  _selectedCategory = null;
                                } else {
                                  _selectedCategory =
                                      categoriesWithOfferId.first;
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
                                  onChanged: isUpdated
                                      ? null
                                      : (newValue) {
                                          setState(() {
                                            _selectedCategory = newValue;
                                            _properties.clear();
                                            if (newValue != null) {
                                              _propertiesFuture =
                                                  _fetchProperties(
                                                    newValue.name,
                                                  );
                                            } else {
                                              _propertiesFuture = null;
                                            }
                                          });
                                        },
                                  enabled: !isUpdated,
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
                              itemCount: isUpdated
                                  ? _images.length
                                  : _images.length + 1,
                              itemBuilder: (context, index) {
                                if (index == _images.length) {
                                  return Padding(
                                    padding: const EdgeInsets.all(8.0),
                                    child: InkWell(
                                      onTap: _pickImage,
                                      child: Container(
                                        width: 100,
                                        decoration: BoxDecoration(
                                          border: Border.all(
                                            color: Colors.grey,
                                          ),
                                          borderRadius: BorderRadius.circular(
                                            8,
                                          ),
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
                          Row(
                            children: [
                              const Text(
                                "Tags",
                                style: TextStyle(
                                  fontSize: 18,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              if (isUpdated)
                                _buildEditIcon(isUpdated, _isTagsEditing, () {
                                  setState(
                                    () => _isTagsEditing = !_isTagsEditing,
                                  );
                                })!,
                            ],
                          ),
                          if (!isUpdated || _isTagsEditing)
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
                                    onDeleted: (!isUpdated || _isTagsEditing)
                                        ? () => _removeTag(tag)
                                        : null,
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

                              return Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    children: [
                                      const Text(
                                        "Category Specific Properties",
                                        style: TextStyle(
                                          fontSize: 18,
                                          fontWeight: FontWeight.bold,
                                        ),
                                      ),
                                      if (isUpdated)
                                        _buildEditIcon(
                                          isUpdated,
                                          _isPropertiesEditing,
                                          () {
                                            setState(
                                              () => _isPropertiesEditing =
                                                  !_isPropertiesEditing,
                                            );
                                          },
                                        )!,
                                    ],
                                  ),
                                  const SizedBox(height: 16),
                                  ...properties.map(
                                    (prop) => PropertyField(
                                      property: prop,
                                      value: _properties[prop.id.toString()],
                                      enabled:
                                          !isUpdated || _isPropertiesEditing,
                                      onChanged: (newValue) {
                                        setState(() {
                                          _properties[prop.id.toString()] =
                                              newValue.toString();
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
                                padding: const EdgeInsets.symmetric(
                                  vertical: 16,
                                ),
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
              },
            ),
          ),
        ),
      ),
    );
  }
}

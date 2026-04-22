import 'package:flutter/material.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/property_definition.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/display_field.dart';

class OfferDetailsPage extends StatefulWidget {
  final int offerId;

  const OfferDetailsPage({super.key, required this.offerId});

  @override
  State<OfferDetailsPage> createState() => _OfferDetailsPageState();
}

class _OfferDetailsPageState extends State<OfferDetailsPage> {
  late Future<(Offer, List<PropertyDefinition>)> _dataFuture;

  @override
  void initState() {
    super.initState();
    _dataFuture = _fetchData();
  }

  Future<(Offer, List<PropertyDefinition>)> _fetchData() async {
    final offer = await _fetchOfferDetails(widget.offerId);
    final properties = await _fetchProperties(offer.category.id);
    return (offer, properties);
  }

  // Mock fetch method - easy to replace with API call later
  Future<Offer> _fetchOfferDetails(int id) async {
    await Future.delayed(const Duration(seconds: 1));

    // Mock data based on OfferDto structure
    return Offer(
      id: id,
      title: "High Performance Gaming Laptop",
      description:
          "A powerful gaming laptop with the latest components, perfect for gaming and professional workloads. Lightly used, excellent condition.",
      price: 4500.0,
      images: [
        "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=800",
        "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=800",
        "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=800",
      ],
      seller: const Seller(id: 1, name: "TechStore Poland"),
      category: Category(
        id: 0,
        name: "Laptops",
        description: "Portable personal computers for mobile use.",
      ),
      tags: ["Gaming", "RTX", "Laptop", "Performance"],
      properties: {
        0: "Intel Core i9-13900HX",
        1: "32",
        2: "1000",
        3: "Windows 11 Home",
        4: "false",
      },
      availability: 5,
      status: OfferStatus.active,
      createdAt: DateTime.now().subtract(const Duration(days: 2)),
      updatedAt: DateTime.now().subtract(const Duration(hours: 5)),
    );
  }

  Future<List<PropertyDefinition>> _fetchProperties(int categoryId) async {
    await Future.delayed(const Duration(milliseconds: 500));
    if (categoryId == 0) {
      // Computers/Laptops
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
      // Apartments
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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const DealmatcherAppBar(),
      body: FutureBuilder<(Offer, List<PropertyDefinition>)>(
        future: _dataFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          }
          if (!snapshot.hasData) {
            return const Center(child: Text('Offer not found'));
          }

          final data = snapshot.data!;
          return _buildOfferDetails(data.$1, data.$2);
        },
      ),
    );
  }

  Widget _buildOfferDetails(
    Offer offer,
    List<PropertyDefinition> propertyDefinitions,
  ) {
    final theme = Theme.of(context);

    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Image Gallery
          _buildImageGallery(offer.images),

          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Title and Price
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Expanded(
                      child: Text(
                        offer.title,
                        style: theme.textTheme.headlineMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    Text(
                      "${offer.price.toStringAsFixed(2)} PLN",
                      style: theme.textTheme.headlineSmall?.copyWith(
                        color: theme.colorScheme.primary,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),

                // Status and Category Chip
                Row(
                  children: [
                    Chip(
                      label: Text(offer.status.name.toUpperCase()),
                      backgroundColor: _getStatusColor(
                        offer.status,
                      ).withValues(alpha: 0.1),
                      labelStyle: TextStyle(
                        color: _getStatusColor(offer.status),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Chip(
                      label: Text(offer.category.name),
                      avatar: const Icon(Icons.category, size: 16),
                    ),
                  ],
                ),
                const SizedBox(height: 16),

                // Description Section
                Text(
                  "Description",
                  style: theme.textTheme.titleLarge?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 8),
                Text(offer.description, style: theme.textTheme.bodyLarge),
                const SizedBox(height: 24),

                // Properties Section
                if (offer.properties.isNotEmpty) ...[
                  Text(
                    "Specifications",
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 12),
                  ...offer.properties.entries.map((entry) {
                    final definition = propertyDefinitions.firstWhere(
                      (d) => d.id == entry.key,
                      orElse: () => PropertyDefinition(
                        id: entry.key,
                        name: "Property ${entry.key}",
                        type: PropertyType.text,
                        options: [],
                      ),
                    );
                    return _buildPropertyRow(definition, entry.value);
                  }),
                  const SizedBox(height: 24),
                ],

                // Seller Info
                Text(
                  "Seller Information",
                  style: theme.textTheme.titleLarge?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 8),
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  leading: const CircleAvatar(child: Icon(Icons.person)),
                  title: Text(offer.seller.name),
                  subtitle: const Text("Verified Seller"),
                ),
                const Divider(),

                // Additional Details
                const SizedBox(height: 16),
                DisplayField(
                  label: "Availability",
                  text: "${offer.availability} items left",
                ),
                DisplayField(
                  label: "Published on",
                  text: _formatDate(offer.createdAt),
                ),
                DisplayField(
                  label: "Last updated",
                  text: _formatDate(offer.updatedAt),
                ),

                // Tags
                if (offer.tags.isNotEmpty) ...[
                  const SizedBox(height: 16),
                  Text("Tags", style: theme.textTheme.titleMedium),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: offer.tags
                        .map((tag) => Chip(label: Text("#$tag")))
                        .toList(),
                  ),
                ],

                const SizedBox(height: 32),

                // Action Buttons
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: () {},
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 16),
                    ),
                    child: const Text(
                      "ADD TO CART",
                      style: TextStyle(fontSize: 18),
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton(
                    onPressed: () {},
                    style: OutlinedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 16),
                    ),
                    child: const Text(
                      "CONTACT SELLER",
                      style: TextStyle(fontSize: 18),
                    ),
                  ),
                ),
                const SizedBox(height: 64),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildImageGallery(List<String> images) {
    if (images.isEmpty) {
      return Container(
        height: 250,
        width: double.infinity,
        color: Colors.grey[300],
        child: const Icon(
          Icons.image_not_supported,
          size: 100,
          color: Colors.grey,
        ),
      );
    }

    return SizedBox(
      height: 300,
      child: PageView.builder(
        controller: PageController(viewportFraction: 0.9),
        itemCount: images.length,
        itemBuilder: (context, index) {
          return Padding(
            padding: const EdgeInsets.symmetric(horizontal: 4.0),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(8),
              child: Image.network(
                images[index],
                fit: BoxFit.contain,
                loadingBuilder: (context, child, loadingProgress) {
                  if (loadingProgress == null) return child;
                  return const Center(child: CircularProgressIndicator());
                },
                errorBuilder: (context, error, stackTrace) =>
                    const Center(child: Icon(Icons.error)),
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildPropertyRow(PropertyDefinition definition, String value) {
    Widget valueWidget;

    final valueLower = value.toLowerCase();
    if (definition.type == PropertyType.boolean) {
      final bool val = valueLower == 'true';
      valueWidget = Icon(
        val ? Icons.check_circle : Icons.cancel,
        color: val ? Colors.green : Colors.red,
        size: 20,
      );
    } else {
      valueWidget = Text(value);
    }

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          Expanded(
            flex: 2,
            child: Text(
              definition.name,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: Colors.grey,
              ),
            ),
          ),
          Expanded(
            flex: 3,
            child: Align(alignment: Alignment.centerLeft, child: valueWidget),
          ),
        ],
      ),
    );
  }

  Color _getStatusColor(OfferStatus status) {
    switch (status) {
      case OfferStatus.active:
        return Colors.green;
      case OfferStatus.deleted:
        return Colors.grey;
      case OfferStatus.sold:
        return Colors.blue;
    }
  }

  String _formatDate(DateTime date) {
    return "${date.day.toString().padLeft(2, '0')}.${date.month.toString().padLeft(2, '0')}.${date.year}";
  }
}

import 'package:flutter/material.dart';
import 'package:frontend/api/api_categories.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';
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
  ApiOffers apiOfferDetails = ApiOffers();
  ApiCategories apiProperties = ApiCategories();

  @override
  void initState() {
    super.initState();
    _dataFuture = _fetchData();
  }

  Future<(Offer, List<PropertyDefinition>)> _fetchData() async {
    final offer = await _fetchOfferDetails(widget.offerId);
    final properties = await _fetchProperties(offer.category);
    return (offer, properties);
  }

  Future<Offer> _fetchOfferDetails(int id) async {
    var offer = await apiOfferDetails.getOffer(id);

    if (offer == null) {
      throw Exception('Offer does not exist.');
    }

    return offer;
  }

  Future<List<PropertyDefinition>> _fetchProperties(Category category) async {
    var properties = await apiProperties.getPropertyDefinitions(category.name);
    return properties;
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
            return Center(
              child: Text(
                'Error: ${snapshot.error}'.toString().replaceAll(
                  'Exception: ',
                  '',
                ),
              ),
            );
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

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/offer_filter_widget.dart';
import 'package:frontend/models/category.dart';

class OffersSwipingPage extends StatefulWidget {
  const OffersSwipingPage({super.key});

  @override
  State<OffersSwipingPage> createState() => _OffersSwipingPageState();
}

class _OffersSwipingPageState extends State<OffersSwipingPage> {
  List<Offer> _offers = [];
  bool _isLoading = true;
  Map<String, dynamic> _currentFilters = {};

  @override
  void initState() {
    super.initState();
    _fetchOffers();
  }

  Future<void> _fetchOffers() async {
    setState(() => _isLoading = true);
    try {
      final results = await MockApi.searchOffers(_currentFilters);
      setState(() {
        _offers = results;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
    }
  }

  Future<void> _addToCart(Offer offer) async {
    // this snackbar may be removed later while integrating with api
    ScaffoldMessenger.of(context).clearSnackBars();
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Added to cart: ${offer.title}'),
        backgroundColor: Colors.green,
        duration: const Duration(seconds: 2),
      ),
    );

    // here will be adding to cart via api
  }

  void _openFilters() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) {
        return DraggableScrollableSheet(
          initialChildSize: 0.85,
          maxChildSize: 0.95,
          minChildSize: 0.5,
          builder: (_, controller) {
            return Container(
              decoration: BoxDecoration(
                color: Theme.of(context).scaffoldBackgroundColor,
                borderRadius: const BorderRadius.vertical(
                  top: Radius.circular(16),
                ),
              ),
              child: ListView(
                controller: controller,
                children: [
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Center(
                      child: Container(
                        width: 40,
                        height: 5,
                        decoration: BoxDecoration(
                          color: Colors.grey[600],
                          borderRadius: BorderRadius.circular(10),
                        ),
                      ),
                    ),
                  ),
                  OfferFilterWidget(
                    onFilterChanged: (filters) {
                      _currentFilters = filters;
                      _fetchOffers();
                    },
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      extendBodyBehindAppBar: true,
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.filter_list, color: Colors.white, size: 30),
            onPressed: _openFilters,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _offers.isEmpty
          ? const Center(
              child: Text(
                "No offers matching the criteria. You've seen everything!",
              ),
            )
          : Dismissible(
              key: ValueKey(_offers.first.id),
              direction: DismissDirection.horizontal,

              background: Container(
                color: Colors.green.withValues(alpha: 0.8),
                alignment: Alignment.centerLeft,
                padding: const EdgeInsets.only(left: 40),
                child: const Icon(
                  Icons.shopping_cart,
                  color: Colors.white,
                  size: 60,
                ),
              ),

              secondaryBackground: Container(
                color: Colors.red.withValues(alpha: 0.8),
                alignment: Alignment.centerRight,
                padding: const EdgeInsets.only(right: 40),
                child: const Icon(Icons.close, color: Colors.white, size: 60),
              ),

              onDismissed: (direction) {
                final swipedOffer = _offers.first;

                if (direction == DismissDirection.startToEnd) {
                  _addToCart(swipedOffer);
                }

                setState(() {
                  _offers.removeAt(0);
                });
              },
              child: GestureDetector(
                onTap: () => context.push('/offer/${_offers.first.id}'),
                child: _buildOfferPage(_offers.first),
              ),
            ),
    );
  }

  Widget _buildOfferPage(Offer offer) {
    return Stack(
      fit: StackFit.expand,
      children: [
        Image.network(
          offer.images.isNotEmpty
              ? offer.images.first
              : 'https://media1.tenor.com/m/j4MNRU71aeUAAAAC/just-stop-stop-it.gif',
          fit: BoxFit.cover,
          errorBuilder: (context, error, stackTrace) =>
              Container(color: Colors.grey[800]),
        ),
        Positioned(
          bottom: 0,
          left: 0,
          right: 0,
          child: Container(
            height: 250,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.bottomCenter,
                end: Alignment.topCenter,
                colors: [
                  Colors.black.withValues(alpha: 0.9),
                  Colors.black.withValues(alpha: 0.6),
                  Colors.transparent,
                ],
              ),
            ),
          ),
        ),
        Positioned(
          bottom: 40,
          left: 20,
          right: 20,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 10,
                  vertical: 4,
                ),
                decoration: BoxDecoration(
                  color: Theme.of(context).primaryColor,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  offer.category.name,
                  style: const TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              const SizedBox(height: 8),
              Text(
                offer.title,
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 28,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 4),
              Text(
                "${offer.price.toStringAsFixed(2)} zł",
                style: const TextStyle(
                  color: Colors.greenAccent,
                  fontSize: 22,
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                offer.description,
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(color: Colors.white70, fontSize: 16),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class MockApi {
  static final List<Category> _categories = [
    Category(
      id: 0,
      name: "Computers",
      description: "PC, Laptops and Notebooks",
    ),
    Category(
      id: 1,
      name: "Apartments",
      description: "Apartments for rent or for sale",
    ),
  ];

  static final List<Offer> _allOffers = [
    Offer(
      id: 1,
      title: "Apple Mac Pro",
      description: "Świetna tarka do sera w przystępniej cenie, stan igła.",
      price: 262000.0,
      images: ["https://txesmika.com/3376-large_default/mac-pro-m2-ultra.jpg"],
      seller: const Seller(id: 1, name: "Tadeusz Norek"),
      category: _categories[0],
      tags: ["apple", "mac", "pro"],
      properties: {0: "Mac Pro", 1: "128", 2: "2048", 3: "MacOS", 4: "true"},
      availability: 1,
      status: OfferStatus.active,
      createdAt: DateTime.now().subtract(const Duration(days: 2)),
      updatedAt: DateTime.now(),
    ),
    Offer(
      id: 2,
      title: "ThinkPad T500",
      description:
          "Biznesowy sprzęt dla profesjonalistów nie zwracających uwagi na drobne zarysowania. Arch na spokojnie na nim pójdzie.",
      price: 420.0,
      images: [
        "https://preview.redd.it/anxious-that-my-stand-will-scratch-the-thinkpad-v0-q2ne3m8ggikg1.png?width=1080&format=png&auto=webp&s=8bd6b81827d5c41f58dc77594bb8a27aa9b1374a",
      ],
      seller: const Seller(id: 2, name: "Karol Krawczyk"),
      category: _categories[0],
      tags: ["lenovo", "thinkpad", "linux"],
      properties: {0: "T500", 1: "2", 2: "128", 3: "Linux", 4: "false"},
      availability: 5,
      status: OfferStatus.active,
      createdAt: DateTime.now().subtract(const Duration(days: 5)),
      updatedAt: DateTime.now(),
    ),
    Offer(
      id: 3,
      title: "Kawalerka na Bemowie",
      description:
          "Przestronna kawalerka o 9m2 na Bemowie z widokiem na świecące całą dobę logo Biedronki.",
      price: 850000.0,
      images: [
        "https://wykop.pl/cdn/c3397993/5fb02469f44bec7c07524bb0f86d3da6ed87dd74e5a14bec271200a4418d0c79,w300h194.jpg",
      ],
      seller: const Seller(
        id: 3,
        name: "Agencja Nieruchomości 'Tanie Mieszkanie'",
      ),
      category: _categories[1],
      tags: ["Bemowo", "używane", "kawalerka", "warszawa"],
      properties: {5: "1", 6: "4", 7: "false", 8: "Gas"},
      availability: 1,
      status: OfferStatus.active,
      createdAt: DateTime.now().subtract(const Duration(days: 10)),
      updatedAt: DateTime.now(),
    ),
    Offer(
      id: 4,
      title: "Apartament w Suwałkach",
      description:
          "Drogi, luksusowy apartament o 120m2 na Północy w Suwałkach.",
      price: 600000.0,
      images: [
        "https://storage.googleapis.com/bd-pl-01/buildings-v2/2560x1920/47700.jpg",
      ],
      seller: const Seller(id: 4, name: "Optimus Prime"),
      category: _categories[1],
      tags: ["drogie", "luksusowe", "apartament"],
      properties: {5: "4", 6: "2", 7: "true", 8: "Central"},
      availability: 1,
      status: OfferStatus.active,
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
    ),
  ];

  static Future<List<Offer>> searchOffers(Map<String, dynamic> filters) async {
    await Future.delayed(const Duration(milliseconds: 800));

    final String phrase = (filters['phrase'] as String?)?.toLowerCase() ?? '';
    final double? priceMin = filters['priceMin'] as double?;
    final double? priceMax = filters['priceMax'] as double?;
    final List<String> tags = (filters['tags'] as List<String>?) ?? [];
    final int? categoryId = filters['categoryId'] as int?;
    final Map<int, List<String>> properties =
        (filters['properties'] as Map<int, List<String>>?) ?? {};

    return _allOffers.where((offer) {
      if (categoryId != null && offer.category.id != categoryId) return false;
      if (priceMin != null && offer.price < priceMin) return false;
      if (priceMax != null && offer.price > priceMax) return false;
      if (phrase.isNotEmpty &&
          !offer.title.toLowerCase().contains(phrase) &&
          !offer.description.toLowerCase().contains(phrase)) {
        return false;
      }
      if (tags.isNotEmpty) {
        bool hasAnyTag = tags.any((tag) => offer.tags.contains(tag));
        if (!hasAnyTag) return false;
      }

      for (var entry in properties.entries) {
        final propId = entry.key;
        final filterValues = entry.value;

        if (filterValues.isEmpty) continue;

        if (filterValues.length == 2 &&
            (filterValues[0].isNotEmpty || filterValues[1].isNotEmpty)) {
          final offerVal = double.tryParse(offer.properties[propId] ?? '');
          final minVal = double.tryParse(filterValues[0]);
          final maxVal = double.tryParse(filterValues[1]);

          if (offerVal != null) {
            if (minVal != null && offerVal < minVal) return false;
            if (maxVal != null && offerVal > maxVal) return false;
          }
        } else if (filterValues.isNotEmpty && filterValues.first.isNotEmpty) {
          if (!filterValues.contains(offer.properties[propId])) return false;
        }
      }

      return true;
    }).toList();
  }
}

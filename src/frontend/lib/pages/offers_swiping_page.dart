import 'package:flutter/material.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/api/models/offer_search_request.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/offer_filter_widget.dart';

class OffersSwipingPage extends StatefulWidget {
  const OffersSwipingPage({super.key});

  @override
  State<OffersSwipingPage> createState() => _OffersSwipingPageState();
}

class _OffersSwipingPageState extends State<OffersSwipingPage> {
  List<Offer> _offers = [];
  bool _isLoading = true;
  Map<String, dynamic> _currentFilters = {};
  static int maxInt = 9007199254740991;
  static int limit = 10;

  @override
  void initState() {
    super.initState();
    _fetchOffers();
  }

  Future<void> _fetchOffers() async {
    setState(() => _isLoading = true);

    try {
      final request = OfferSearchRequest(
        categoryId: _currentFilters['categoryId'],
        minPrice: _currentFilters['minPrice'] ?? 0,
        maxPrice: _currentFilters['maxPrice'] ?? maxInt,
        tags: _currentFilters['tags'] ?? [],
        properties: _currentFilters['properties'] ?? {},
        searchPhrase: _currentFilters['searchPhrase'] ?? '',
        limit: limit,
      );

      final results = await ApiOffers().searchOffers(request);
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

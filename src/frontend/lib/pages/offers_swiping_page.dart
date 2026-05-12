import 'package:flutter/material.dart';
import 'package:frontend/api/api_cart.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/api/models/offer_search_request.dart';
import 'package:frontend/widgets/placeholder_image_widget.dart';
import 'package:go_router/go_router.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/offer_filter_widget.dart';

class OffersSwipingPage extends StatefulWidget {
  const OffersSwipingPage({super.key});

  @override
  State<OffersSwipingPage> createState() => _OffersSwipingPageState();
}

class _OffersSwipingPageState extends State<OffersSwipingPage> {
  final apiCart = ApiCart();
  List<Offer> _offersToSwipe = [];
  List<Offer> _offers = [];
  bool _isLoading = true;
  Map<String, dynamic> _currentFilters = {};
  static double maxPriceLimit = 9007199254740991.0;
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
        minPrice: (_currentFilters['minPrice'] as num?)?.toDouble() ?? 0.0,
        maxPrice:
            (_currentFilters['maxPrice'] as num?)?.toDouble() ?? maxPriceLimit,
        tags:
            (_currentFilters['tags'] as List<dynamic>?)?.cast<String>() ??
            <String>[],
        properties:
            (_currentFilters['properties'] as Map<dynamic, dynamic>?)
                ?.cast<String, List<String>>() ??
            <String, List<String>>{},
        searchPhrase: _currentFilters['searchPhrase'] ?? '',
        limit: limit,
      );

      final results = await ApiOffers().searchOffers(request);
      setState(() {
        _offers = results;
        _offersToSwipe = results;
        _isLoading = false;
      });
    } catch (e) {
      setState(() => _isLoading = false);
    }
  }

  Future<bool> _addToCart(BuildContext context, Offer offer) async {
    try {
      await apiCart.addToCart(offer.id);
    } catch (e) {
      if (context.mounted) {
        ScaffoldMessenger.of(context).clearSnackBars();
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              'Error: ${e.toString().trim().replaceFirst('Exception: ', '')}',
            ),
            backgroundColor: Colors.red.shade700,
          ),
        );
        return false;
      }
    }

    return true;
  }

  void _openFilters() {
    Map<String, dynamic>? pendingFilters;
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
                    initialFilters: _currentFilters,
                    onFilterChanged: (filters) {
                      pendingFilters = filters;
                    },
                  ),
                ],
              ),
            );
          },
        );
      },
    ).then((_) {
      if (pendingFilters != null) {
        setState(() {
          _currentFilters = pendingFilters!;
        });
        _fetchOffers();
      }
    });
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
              key: ValueKey(_offersToSwipe.first.id),
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

              confirmDismiss: (direction) async {
                bool dismissed = true;

                final swipedOffer = _offersToSwipe.first;

                if (direction == DismissDirection.startToEnd) {
                  dismissed = await _addToCart(context, swipedOffer);
                }

                if (dismissed) {
                  setState(() {
                    _offersToSwipe.removeAt(0);
                  });

                  if (_offersToSwipe.isEmpty) {
                    await _fetchOffers();
                  }
                }

                return dismissed;
              },
              child: GestureDetector(
                onTap: () => context.push('/offer/${_offersToSwipe.first.id}'),
                child: _buildOfferPage(_offersToSwipe.first),
              ),
            ),
    );
  }

  Widget _buildOfferPage(Offer offer) {
    return Stack(
      fit: StackFit.expand,
      children: [
        offer.images.isNotEmpty
            ? Image.network(
                offer.images.first,
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) => Container(
                  color: Colors.grey[200],
                  alignment: Alignment.center,
                  child: const Icon(
                    Icons.broken_image,
                    color: Colors.grey,
                    size: 40,
                  ),
                ),
              )
            : placeholderImageWidget(),
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

import 'package:flutter/material.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:go_router/go_router.dart';

class MyOffersPage extends StatefulWidget {
  const MyOffersPage({super.key, this.offersFuture});

  final Future<List<Offer>>? offersFuture;

  @override
  State<StatefulWidget> createState() => _MyOffersPageState();
}

class _MyOffersPageState extends State<MyOffersPage> {
  late Future<List<Offer>> _dataFuture;
  final apiOffers = ApiOffers();

  @override
  void initState() {
    super.initState();
    _dataFuture = widget.offersFuture ?? _fetchData();
  }

  Future<List<Offer>> _fetchData() async {
    final offers = await _fetchOffers();
    return offers;
  }

  Future<List<Offer>> _fetchOffers() async {
    return await apiOffers.getMyOffers();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: FutureBuilder(
        future: _dataFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshot.hasData && snapshot.data != null) {
            return _buildMyOfferList(snapshot.data!);
          }
          return Center(child: Text('Error: ${snapshot.error.toString().replaceAll('Exception: ', '')}'));
        },
      ),
    );
  }

  Widget _buildMyOfferList(List<Offer> offers) {
    final theme = Theme.of(context);
    return CustomScrollView(
      slivers: [
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.only(left: 16, top: 16, bottom: 32),
            child: Text(
              'My Offers',
              style: theme.textTheme.displaySmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ),
        SliverList.builder(
          itemCount: offers.length,
          itemBuilder: (context, index) {
            final offer = offers[index];
            return Card(
              child: SizedBox(
                height: 120,
                child: Row(
                  children: [
                    AspectRatio(
                      aspectRatio: 1,
                      child: Padding(
                        padding: const EdgeInsets.all(8),
                        child: ClipRRect(
                          borderRadius: BorderRadius.circular(8),
                          child: Image.network(
                            offer.images.first,
                            fit: BoxFit.cover,
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Padding(
                        padding: const EdgeInsets.symmetric(vertical: 8),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(
                              offer.title,
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                              style: theme.textTheme.titleLarge?.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            Text(
                              'Category: ${offer.category.name}',
                              style: theme.textTheme.bodyMedium,
                            ),
                            Text(
                              'Price: ${offer.price.toStringAsFixed(2)}',
                              style: theme.textTheme.bodyMedium?.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    Padding(
                      padding: EdgeInsets.symmetric(horizontal: 12),
                      child: IconButton(
                        onPressed: () {
                          context.push('/update-offer/${offer.id}');
                        },
                        icon: Icon(Icons.arrow_right_alt),
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        ),
      ],
    );
  }
}

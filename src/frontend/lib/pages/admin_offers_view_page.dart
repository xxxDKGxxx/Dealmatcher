import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/placeholder_image_widget.dart';

class AdminOffersViewPage extends StatefulWidget {
  const AdminOffersViewPage({super.key, this.offersFuture});

  final Future<List<Offer>>? offersFuture;

  @override
  State<StatefulWidget> createState() => _AdminOffersViewPageState();
}

class _AdminOffersViewPageState extends State<AdminOffersViewPage> {
  late Future<List<Offer>> _dataFuture;
  final apiAdmin = ApiAdmin();
  final apiProfile = ApiProfile();
  final apiOffer = ApiOffers();

  int page = 0;
  int pages = 1;
  int limit = 16;

  @override
  void initState() {
    super.initState();
    _dataFuture = widget.offersFuture ?? _fetchData();
  }

  Future<List<Offer>> _fetchData() async {
    final user = await apiProfile.getProfile();
    final offers = await _fetchOffers(user);
    return offers;
  }

  Future<List<Offer>> _fetchOffers(User user) async {
    final addOffers = await apiAdmin.getOffers(
      page: page,
      limit: limit,
      status: user.status.name.trim().toLowerCase(),
    );
    page = addOffers.page;
    pages = addOffers.pages;
    return addOffers.items;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: FutureBuilder(
        future: _dataFuture,
        builder: (context, snapshotOffers) {
          if (snapshotOffers.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }
          if (snapshotOffers.hasData && snapshotOffers.data != null) {
            return _buildOfferList(snapshotOffers.data!);
          }
          return Center(
            child: Text(
              snapshotOffers.error.toString().trim().replaceAll(
                'Exception: ',
                '',
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildOfferList(List<Offer> offers) {
    final theme = Theme.of(context);
    return CustomScrollView(
      slivers: [
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.only(left: 16, top: 16, bottom: 32),
            child: Text(
              'Offers',
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
                          child: offer.images.isNotEmpty
                              ? Image.network(
                                  offer.images.first,
                                  fit: BoxFit.cover,
                                )
                              : placeholderImageWidget(),
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
                            Text(
                              'Seller: ${offer.seller.name}',
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
                        onPressed: () async {
                          await apiOffer.deleteOffer(offer.id);
                          setState(() {
                            _dataFuture = widget.offersFuture ?? _fetchData();
                          });
                        },
                        icon: Icon(Icons.delete_rounded),
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        ),
        Padding(
          padding: EdgeInsetsGeometry.symmetric(vertical: 16, horizontal: 8),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              IconButton(
                onPressed: () {
                  setState(() {
                    page = page < 1 ? 0 : page--;
                    _dataFuture = widget.offersFuture ?? _fetchData();
                  });
                },
                icon: Icon(Icons.arrow_back),
              ),
              Text('Page: ${page + 1}/$pages'),
              IconButton(
                onPressed: () {
                  setState(() {
                    page = page >= pages ? pages - 1 : page++;
                    _dataFuture = widget.offersFuture ?? _fetchData();
                  });
                },
                icon: Icon(Icons.arrow_forward),
              ),
              FormField(
                builder: (state) {
                  return Row(
                    children: [
                      Text('Limit:'),
                      SizedBox(
                        width: 100,
                        child: TextFormField(
                          initialValue: limit.toString(),
                          keyboardType: TextInputType.number,
                          decoration: const InputDecoration(
                            border: OutlineInputBorder(),
                            hintText: '16',
                          ),
                          onChanged: (value) {
                            final parsed = int.tryParse(value);

                            if (parsed != null) {
                              setState(() {
                                limit = parsed;
                                _dataFuture =
                                    widget.offersFuture ?? _fetchData();
                              });
                            }
                          },
                        ),
                      ),
                    ],
                  );
                },
              ),
            ],
          ),
        ),
      ],
    );
  }
}

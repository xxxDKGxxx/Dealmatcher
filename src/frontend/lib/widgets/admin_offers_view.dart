import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/offer_list_tile.dart';

class AdminOffersView extends StatefulWidget {
  const AdminOffersView({super.key, this.offersFuture});

  final Future<List<Offer>>? offersFuture;

  @override
  State<StatefulWidget> createState() => _AdminOffersViewState();
}

class _AdminOffersViewState extends State<AdminOffersView> {
  late Future<List<Offer>> _dataFuture;
  final apiAdmin = ApiAdmin();
  final apiOffer = ApiOffers();

  int page = 1;
  int pages = 1;
  int limit = 16;

  OfferStatus _selectedStatus = OfferStatus.active;
  late final TextEditingController _limitController;

  @override
  void initState() {
    super.initState();
    _limitController = TextEditingController(text: limit.toString());
    _dataFuture = widget.offersFuture ?? _fetchData();
  }

  @override
  void dispose() {
    _limitController.dispose();
    super.dispose();
  }

  Future<List<Offer>> _fetchData() async {
    final offers = await _fetchOffers();
    return offers;
  }

  Future<List<Offer>> _fetchOffers() async {
    final allOffers = await apiAdmin.getOffers(
      page: page,
      limit: limit,
      status: _selectedStatus.toString().toLowerCase().split('.').last,
    );
    //page = allOffers.page;
    pages = allOffers.pages;
    return allOffers.items;
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return FutureBuilder(
      future: _dataFuture,
      builder: (context, snapshotOffers) {
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
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 8,
                ),
                child: DropdownButtonFormField<OfferStatus>(
                  initialValue: _selectedStatus,
                  decoration: const InputDecoration(
                    labelText: 'Offer Status',
                    border: OutlineInputBorder(),
                  ),
                  items: OfferStatus.values.map((status) {
                    return DropdownMenuItem(
                      value: status,
                      child: Text(status.name),
                    );
                  }).toList(),
                  onChanged: (value) {
                    if (value == null) return;

                    setState(() {
                      _selectedStatus = value;
                      _dataFuture = widget.offersFuture ?? _fetchData();
                    });
                  },
                ),
              ),
            ),
            if (snapshotOffers.connectionState == ConnectionState.waiting) ...[
              SliverFillRemaining(
                child: const Center(child: CircularProgressIndicator()),
              ),
            ] else if (snapshotOffers.hasData &&
                snapshotOffers.data != null) ...[
              SliverList.builder(
                itemCount: snapshotOffers.data!.length,
                itemBuilder: (context, index) {
                  final offer = snapshotOffers.data![index];

                  return offerListTile(
                    offer: offer,
                    theme: theme,
                    onDelete: () async {
                      await apiOffer.deleteOffer(offer.id);
                      setState(() {
                        _dataFuture = widget.offersFuture ?? _fetchData();
                      });
                    },
                    activateOffer: () async {
                      await apiAdmin.activateOffer(offer.id);
                      setState(() {
                        _dataFuture = widget.offersFuture ?? _fetchData();
                      });
                    },
                  );
                },
              ),
            ] else ...[
              SliverToBoxAdapter(
                child: Center(
                  child: Text(
                    snapshotOffers.error.toString().trim().replaceAll(
                      'Exception: ',
                      '',
                    ),
                  ),
                ),
              ),
            ],
            SliverToBoxAdapter(
              child: Padding(
                padding: EdgeInsetsGeometry.symmetric(
                  vertical: 16,
                  horizontal: 8,
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    IconButton(
                      onPressed: () {
                        setState(() {
                          page = page <= 1 ? 1 : page - 1;
                          _dataFuture = widget.offersFuture ?? _fetchData();
                        });
                      },
                      icon: Icon(Icons.arrow_back),
                    ),
                    Text('Page: $page/$pages'),
                    IconButton(
                      onPressed: () {
                        setState(() {
                          page = page >= pages ? pages : page + 1;
                          _dataFuture = widget.offersFuture ?? _fetchData();
                        });
                      },
                      icon: Icon(Icons.arrow_forward),
                    ),
                    Row(
                      children: [
                        Text('Limit:'),
                        SizedBox(width: 8),
                        SizedBox(
                          width: 100,
                          child: TextFormField(
                            controller: _limitController,
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
                    ),
                  ],
                ),
              ),
            ),
          ],
        );
      },
    );
  }
}

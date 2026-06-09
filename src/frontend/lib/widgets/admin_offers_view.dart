import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/api/api_offers.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/user.dart';
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
  final apiProfile = ApiProfile();
  final apiOffer = ApiOffers();

  int page = 1;
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
    final allOffers = await apiAdmin.getOffers(
      page: page,
      limit: limit,
      status: user.status.name.trim().toLowerCase(),
    );
    page = allOffers.page;
    pages = allOffers.pages;
    return allOffers.items;
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
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

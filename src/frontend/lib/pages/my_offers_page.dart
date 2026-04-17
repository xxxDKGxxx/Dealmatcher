import 'package:flutter/material.dart';
import 'package:frontend/models/category.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';

class MyOffersPage extends StatefulWidget {
  const MyOffersPage({super.key, this.offersFuture});

  final Future<List<Offer>>? offersFuture;

  @override
  State<StatefulWidget> createState() => _MyOffersPageState();
}

class _MyOffersPageState extends State<MyOffersPage> {
  late Future<List<Offer>> _dataFuture;

  @override
  void initState() {
    super.initState();
    _dataFuture = widget.offersFuture ?? _fetchData();
  }

  Future<List<Offer>> _fetchData() async {
    final offers = await _fetchOffers();
    return offers;
  }

  // Mock fetch method - easy to replace with API call later
  Future<List<Offer>> _fetchOffers() async {
    await Future.delayed(const Duration(seconds: 1));

    // Mock data based on OfferDto structure
    return [
      Offer(
        id: 0,
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
          3: "Linux Fedora 43",
          4: "false",
        },
        availability: 5,
        status: OfferStatus.active,
        createdAt: DateTime.now().subtract(const Duration(days: 2)),
        updatedAt: DateTime.now().subtract(const Duration(hours: 5)),
      ),
      Offer(
        id: 0,
        title: "hampter",
        description:
            "A hampter for sleepless nights. Slightly used but in good condition.",
        price: 43.34,
        images: [
          "https://static.wikia.nocookie.net/sprunked-fanon/images/7/71/Staring_Hampter.jpg/revision/latest?cb=20250108234241",
          "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fzwierzaki.pl%2Fwp-content%2Fuploads%2F2022%2F07%2Flysa-swinka-morska-pochodzenie-1024x761.jpg&f=1&nofb=1&ipt=307e6a23f760affb3d4948716418466ce7c7b901b3083e482db5a9346df72bb7",
          "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwallpaperaccess.com%2Ffull%2F8779679.jpg&f=1&nofb=1&ipt=dfc8527dd38f464a0d566b97bb791d24c7515974a3cabc623cad103c43bbe620",
        ],
        seller: const Seller(id: 1, name: "Warsaw Zoo"),
        category: Category(
          id: 0,
          name: "Animals",
          description: "Portable hampters for mobile use.",
        ),
        tags: ["Gaming", "hampter", "animal", "smol"],
        properties: {
          0: "smol",
          1: "animal",
          2: "hampter",
          3: "Linux Fedora 43",
          4: "false",
        },
        availability: 5,
        status: OfferStatus.active,
        createdAt: DateTime.now().subtract(const Duration(days: 2)),
        updatedAt: DateTime.now().subtract(const Duration(hours: 5)),
      ),
    ];
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
          return Center(child: Text('Error: ${snapshot.error}'));
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
                        onPressed: () {},
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

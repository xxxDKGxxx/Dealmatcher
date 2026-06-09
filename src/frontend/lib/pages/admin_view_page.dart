import 'package:flutter/material.dart';
import 'package:frontend/models/ban.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/admin_bans_view.dart';
import 'package:frontend/widgets/admin_offer_activity_view.dart';
import 'package:frontend/widgets/admin_offers_view.dart';
import 'package:frontend/widgets/admin_user_activity_view.dart';
import 'package:frontend/widgets/admin_users_view.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';

class AdminViewPage extends StatelessWidget {
  const AdminViewPage({
    super.key,
    this.offersFuture,
    this.usersFuture,
    this.bansFuture,
  });

  final Future<List<Offer>>? offersFuture;
  final Future<List<User>>? usersFuture;
  final Future<List<Ban>>? bansFuture;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: DefaultTabController(
        length: 5,
        child: Scaffold(
          appBar: PreferredSize(
            preferredSize: const Size.fromHeight(110),
            child: const TabBar(
              isScrollable: true,
              tabs: [
                Tab(icon: Icon(Icons.local_offer), text: 'Offers'),
                Tab(icon: Icon(Icons.people), text: 'Users'),
                Tab(icon: Icon(Icons.local_activity), text: 'User Activities'),
                Tab(icon: Icon(Icons.local_activity), text: 'Offer Activities'),
                Tab(icon: Icon(Icons.block_rounded), text: 'Bans'),
              ],
            ),
          ),
          body: TabBarView(
            children: [
              AdminOffersView(offersFuture: offersFuture),
              AdminUsersView(usersFuture: usersFuture),
              const AdminUserActivityView(),
              const AdminOfferActivityView(),
              AdminBansView(bansFuture: bansFuture),
            ],
          ),
        ),
      ),
    );
  }
}

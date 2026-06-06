import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/ban.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/ban_list_tile.dart';
import 'package:frontend/widgets/user_list_tile.dart';

class AdminBansView extends StatefulWidget {
  const AdminBansView({super.key, this.bansFuture});

  final Future<List<Ban>>? bansFuture;

  @override
  State<StatefulWidget> createState() => _AdminBansViewState();
}

class _AdminBansViewState extends State<AdminBansView> {
  late Future<List<Ban>> _dataFuture;
  final apiAdmin = ApiAdmin();

  @override
  void initState() {
    super.initState();
    _dataFuture = widget.bansFuture ?? _fetchData();
  }

  Future<List<Ban>> _fetchData() async {
    final bans = await apiAdmin.getBans();
    return bans;
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _dataFuture,
      builder: (context, snapshotBans) {
        if (snapshotBans.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshotBans.hasData && snapshotBans.data != null) {
          return _buildBanList(snapshotBans.data!);
        }
        return Center(
          child: Text(
            snapshotBans.error.toString().trim().replaceAll('Exception: ', ''),
          ),
        );
      },
    );
  }

  Widget _buildBanList(List<Ban> bans) {
    final theme = Theme.of(context);
    return CustomScrollView(
      slivers: [
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.only(left: 16, top: 16, bottom: 32),
            child: Text(
              'Bans',
              style: theme.textTheme.displaySmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ),
        SliverList.builder(
          itemCount: bans.length,
          itemBuilder: (context, index) {
            final ban = bans[index];

            return banListTile(ban: ban, theme: theme);
          },
        ),
      ],
    );
  }
}

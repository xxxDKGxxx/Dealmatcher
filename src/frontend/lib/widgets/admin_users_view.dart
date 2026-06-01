import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/user_list_tile.dart';

class AdminUsersView extends StatefulWidget {
  const AdminUsersView({super.key, this.usersFuture});

  final Future<List<User>>? usersFuture;

  @override
  State<StatefulWidget> createState() => _AdminUsersViewState();
}

class _AdminUsersViewState extends State<AdminUsersView> {
  late Future<List<User>> _dataFuture;
  final apiAdmin = ApiAdmin();
  final apiProfile = ApiProfile();

  int page = 0;
  int pages = 1;
  int limit = 16;

  @override
  void initState() {
    super.initState();
    _dataFuture = widget.usersFuture ?? _fetchData();
  }

  Future<List<User>> _fetchData() async {
    final user = await apiProfile.getProfile();
    final users = await _fetchUsers(user);
    return users;
  }

  Future<List<User>> _fetchUsers(User user) async {
    final allUsers = await apiAdmin.getUsers(
      page: page,
      limit: limit,
      status: user.status.name.trim().toLowerCase(),
    );
    page = allUsers.page;
    pages = allUsers.pages;
    return allUsers.items;
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _dataFuture,
      builder: (context, snapshotUsers) {
        if (snapshotUsers.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshotUsers.hasData && snapshotUsers.data != null) {
          return _buildUserList(snapshotUsers.data!);
        }
        return Center(
          child: Text(
            snapshotUsers.error.toString().trim().replaceAll('Exception: ', ''),
          ),
        );
      },
    );
  }

  Widget _buildUserList(List<User> users) {
    final theme = Theme.of(context);
    return CustomScrollView(
      slivers: [
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.only(left: 16, top: 16, bottom: 32),
            child: Text(
              'Users',
              style: theme.textTheme.displaySmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ),
        SliverList.builder(
          itemCount: users.length,
          itemBuilder: (context, index) {
            final user = users[index];

            return userListTile(user: user, theme: theme);
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
                    _dataFuture = widget.usersFuture ?? _fetchData();
                  });
                },
                icon: Icon(Icons.arrow_back),
              ),
              Text('Page: ${page + 1}/$pages'),
              IconButton(
                onPressed: () {
                  setState(() {
                    page = page >= pages ? pages - 1 : page++;
                    _dataFuture = widget.usersFuture ?? _fetchData();
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
                                    widget.usersFuture ?? _fetchData();
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

import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
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

  int page = 1;
  int pages = 1;
  int limit = 16;

  UserStatus _selectedStatus = UserStatus.active;
  late final TextEditingController _limitController;

  @override
  void initState() {
    super.initState();
    _limitController = TextEditingController(text: limit.toString());
    _dataFuture = widget.usersFuture ?? _fetchData();
  }

  Future<List<User>> _fetchData() async {
    final users = await _fetchUsers();
    return users;
  }

  Future<List<User>> _fetchUsers() async {
    final allUsers = await apiAdmin.getUsers(
      page: page,
      limit: limit,
      status: _selectedStatus.toString().toLowerCase().split('.').last,
    );
    //page = allUsers.page;
    pages = allUsers.pages;
    return allUsers.items;
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return FutureBuilder(
      future: _dataFuture,
      builder: (context, snapshotUsers) {
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
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 8,
                ),
                child: DropdownButtonFormField<UserStatus>(
                  initialValue: _selectedStatus,
                  decoration: const InputDecoration(
                    labelText: 'User Status',
                    border: OutlineInputBorder(),
                  ),
                  items: UserStatus.values.map((status) {
                    return DropdownMenuItem(
                      value: status,
                      child: Text(status.name),
                    );
                  }).toList(),
                  onChanged: (value) {
                    if (value == null) return;

                    setState(() {
                      _selectedStatus = value;
                      _dataFuture = widget.usersFuture ?? _fetchData();
                    });
                  },
                ),
              ),
            ),
            if (snapshotUsers.connectionState == ConnectionState.waiting) ...[
              const SliverFillRemaining(
                child: Center(child: CircularProgressIndicator()),
              ),
            ] else if (snapshotUsers.hasData && snapshotUsers.data != null) ...[
              SliverList.builder(
                itemCount: snapshotUsers.data!.length,
                itemBuilder: (context, index) {
                  final user = snapshotUsers.data![index];
                  return userListTile(
                    user: user,
                    theme: theme,
                    context: context,
                  );
                },
              ),
            ] else ...[
              SliverToBoxAdapter(
                child: Center(
                  child: Text(
                    snapshotUsers.error.toString().trim().replaceAll(
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
                          _dataFuture = widget.usersFuture ?? _fetchData();
                        });
                      },
                      icon: Icon(Icons.arrow_back),
                    ),
                    Text('Page: $page/$pages'),
                    IconButton(
                      onPressed: () {
                        setState(() {
                          page = page >= pages ? pages : page + 1;
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
            ),
          ],
        );
      },
    );
  }
}

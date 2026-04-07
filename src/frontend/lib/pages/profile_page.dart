import 'package:flutter/material.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/display_field.dart';
import 'package:frontend/widgets/menu_drawer.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<StatefulWidget> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final Future<User> _futureUser = Future<User>(() async {
    return await ApiProfile().getProfile();
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      drawer: MenuDrawer(),
      body: FutureBuilder(
        future: _futureUser,
        builder: (context, snapshot) {
          if (snapshot.hasData) {
            if (snapshot.data != null) {
              return UserDataList(user: snapshot.data!);
            }
            return Center(child: Text('Error loading user data.'));
          } else if (snapshot.hasError) {
            Future.delayed(Duration(seconds: 1), () {
              if (context.mounted) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text(
                      snapshot.error.toString().replaceAll('Exception: ', ''),
                    ),
                    backgroundColor: Colors.red.shade700,
                  ),
                );
              }
            });
            return Center(child: Text('Error loading user data.'));
          } else {
            return Center(child: CircularProgressIndicator());
          }
        },
      ),
    );
  }
}

class UserDataList extends StatelessWidget {
  const UserDataList({super.key, required this.user});

  final User user;
  String get status =>
      '${user.status.toString().split('.').last[0].toUpperCase()}${user.status.toString().split('.').last.substring(1)}';
  String get data =>
      '${user.createdAt.day.toString().padLeft(2, '0')}.${user.createdAt.month.toString().padLeft(2, '0')}.${user.createdAt.year}';

  @override
  Widget build(BuildContext context) {
    return Center(
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 700),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: CustomScrollView(
            slivers: [
              SliverList.list(
                children: [
                  const SizedBox(height: 32),
                  const Text(
                    'Profile',
                    style: TextStyle(fontSize: 32, fontWeight: FontWeight.bold),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 30),
                  DisplayField(label: 'Name', text: user.name),
                  DisplayField(label: 'Surname', text: user.surname),
                  DisplayField(label: 'Email', text: user.email),
                  DisplayField(label: 'Status', text: status),
                  DisplayField(label: 'User since', text: data),
                  SizedBox(height: 48),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

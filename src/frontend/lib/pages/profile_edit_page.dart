import 'package:flutter/material.dart';
import 'package:frontend/api/api_profile.dart';
import 'package:frontend/models/user.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/form_fields.dart';
import 'package:go_router/go_router.dart';

class ProfileEditPage extends StatefulWidget {
  const ProfileEditPage({super.key});

  @override
  State<StatefulWidget> createState() => _ProfileEditPageState();
}

class _ProfileEditPageState extends State<ProfileEditPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _surnameController = TextEditingController();

  final Future<User> _futureUser = Future<User>(() async {
    return await ApiProfile().getProfile();
  });

  Future<void> _update(BuildContext context) async {
    await ApiProfile().updateProfile(
      _nameController.text,
      _surnameController.text,
    );

    if (context.mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Successfully changed profile data.')),
      );
      context.replace('/profile');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: FutureBuilder(
        future: _futureUser,
        builder: (context, snapshot) {
          if (snapshot.hasError) {
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
          } else if (snapshot.hasData) {
            _nameController.text = snapshot.data!.name;
            _surnameController.text = snapshot.data!.surname;
            return Center(
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 700),
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Form(
                    key: _formKey,
                    child: CustomScrollView(
                      slivers: [
                        SliverList.list(
                          children: [
                            const SizedBox(height: 32),
                            const Text(
                              "Edit Profile",
                              style: TextStyle(
                                fontSize: 32,
                                fontWeight: FontWeight.bold,
                              ),
                              textAlign: TextAlign.center,
                            ),
                            const SizedBox(height: 30),
                            nonEmptyTextFormField(
                              controller: _nameController,
                              text: 'Name',
                            ),
                            const SizedBox(height: 16),
                            nonEmptyTextFormField(
                              controller: _surnameController,
                              text: 'Surname',
                            ),
                            const SizedBox(height: 16),
                            ElevatedButton(
                              onPressed: () => _update(context),
                              child: const Text("Update"),
                            ),
                            const SizedBox(height: 64),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            );
          } else {
            return Center(child: CircularProgressIndicator());
          }
        },
      ),
    );
  }
}

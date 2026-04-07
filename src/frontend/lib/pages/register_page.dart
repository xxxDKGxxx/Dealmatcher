import 'package:flutter/material.dart';
import 'package:frontend/api/api_register.dart';
import 'package:frontend/widgets/form_fields.dart';
import 'package:go_router/go_router.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<StatefulWidget> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _surnameController = TextEditingController();

  final _apiRegister = ApiRegister();

  @override
  void initState() {
    super.initState();
  }

  Future<void> _register(BuildContext context) async {
    if (_formKey.currentState!.validate()) {
      String name = _nameController.text;
      String surname = _surnameController.text;
      String email = _emailController.text;
      String password = _passwordController.text;

      try {
        await _apiRegister.register(email, password, name, surname);
        if (context.mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(
                'Successfully registered new account with mail $email',
              ),
            ),
          );
          context.go('/');
        }
      } catch (e) {
        if (context.mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(e.toString().replaceAll('Exception: ', '')),
              backgroundColor: Colors.red.shade700,
            ),
          );
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Scaffold(
      appBar: AppBar(
        title: Text('DealMatcher'),
        backgroundColor: theme.colorScheme.inversePrimary,
      ),
      body: Center(
        child: ConstrainedBox(
          constraints: BoxConstraints(maxWidth: 700),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Form(
              key: _formKey,
              child: CustomScrollView(
                slivers: [
                  SliverList.list(
                    children: [
                      const SizedBox(height: 32),
                      // Register title
                      const Text(
                        "Register",
                        style: TextStyle(
                          fontSize: 32,
                          fontWeight: FontWeight.bold,
                        ),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 30),

                      // Personal info
                      nonEmptyTextFormField(
                        controller: _nameController,
                        text: 'Name',
                      ),
                      const SizedBox(height: 16),
                      nonEmptyTextFormField(
                        controller: _surnameController,
                        text: 'Surname',
                      ),
                      const SizedBox(height: 48),

                      // Account info
                      emailFormField(controller: _emailController),
                      const SizedBox(height: 16),
                      passwordFormField(controller: _passwordController),
                      const SizedBox(height: 16),
                      nonEmptyTextFormField(
                        controller: _confirmPasswordController,
                        text: 'Confirm Password',
                        obscureText: true,
                        additionalValidator: (s) =>
                            s != _passwordController.text,
                        errorText: 'Repeated password is different',
                      ),
                      const SizedBox(height: 24),

                      // Button
                      ElevatedButton(
                        onPressed: () => _register(context),
                        child: const Text("Register"),
                      ),
                      const SizedBox(height: 64),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

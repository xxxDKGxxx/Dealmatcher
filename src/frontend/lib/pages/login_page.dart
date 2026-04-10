import 'package:flutter/material.dart';
<<<<<<< HEAD
import 'package:frontend/api/api_auth.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
=======
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
import 'package:frontend/widgets/form_fields.dart';
import 'package:go_router/go_router.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
<<<<<<< HEAD
  final ApiAuth _authApi = ApiAuth();

  Future<void> _login(BuildContext context) async {
=======

  void _login() {
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
    if (_formKey.currentState!.validate()) {
      String email = _emailController.text;
      String password = _passwordController.text;

<<<<<<< HEAD
      try {
        await _authApi.login(email, password);
        if (context.mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Logged in successfully with mail $email')),
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
=======
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Logged in: $email, $password')));
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
    }
  }

  @override
  Widget build(BuildContext context) {
<<<<<<< HEAD
    return Scaffold(
      appBar: DealmatcherAppBar(),
=======
    final theme = Theme.of(context);
    return Scaffold(
      appBar: AppBar(
        title: Text('DealMatcher'),
        backgroundColor: theme.colorScheme.inversePrimary,
      ),
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
      body: Center(
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
                      // Login title
                      const Text(
                        "Login",
                        style: TextStyle(
                          fontSize: 32,
                          fontWeight: FontWeight.bold,
                        ),
                        textAlign: TextAlign.center,
                      ),
                      const SizedBox(height: 30),

                      // Login info
                      emailFormField(controller: _emailController),
                      const SizedBox(height: 16),
                      passwordFormField(controller: _passwordController),
                      const SizedBox(height: 16),
                      ElevatedButton(
<<<<<<< HEAD
                        onPressed: () => _login(context),
=======
                        onPressed: _login,
>>>>>>> 080d380cd0cecd5435e22b722f31ff8c34f0f5de
                        child: const Text("Login"),
                      ),
                      const SizedBox(height: 16),
                      Center(
                        child: InkWell(
                          onTap: () => context.push('/register'),
                          child: Text("Don't have account? Click here."),
                        ),
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

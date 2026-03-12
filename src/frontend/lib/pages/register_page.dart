import 'package:flutter/material.dart';

class RegisterPage extends StatefulWidget {
  const RegisterPage({super.key});

  @override
  State<StatefulWidget> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final TextEditingController _loginController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _surnameController = TextEditingController();
  final TextEditingController _companyController = TextEditingController();
  final TextEditingController _birthdayDayController = TextEditingController();
  final TextEditingController _birthdayMonthController =
      TextEditingController();
  final TextEditingController _birthdayYearController = TextEditingController();
  DateTime? _birthdayDate;

  @override
  void initState() {
    super.initState();
  }

  void _register() {
    if (_formKey.currentState!.validate()) {
      String name = _nameController.text;
      String surname = _surnameController.text;
      String birthday = _birthdayDate!.toIso8601String();
      String company = _companyController.text;
      String login = _loginController.text;
      String email = _emailController.text;
      String password = _passwordController.text;

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Registered: $name, $surname, $birthday, $company, $login, $email, $password',
          ),
        ),
      );
    }
  }

  Future<void> pickDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
    );

    if (picked != null) {
      _birthdayDate = picked;

      _birthdayDayController.text = _birthdayDate!.day.toString().padLeft(
        2,
        '0',
      );
      _birthdayMonthController.text = _birthdayDate!.month.toString().padLeft(
        2,
        '0',
      );
      _birthdayYearController.text = _birthdayDate!.year.toString();

      setState(() {});
    }
  }

  Widget nonEmptyTextFormField({
    required TextEditingController controller,
    required String text,
    bool Function(String)? additionalValidator,
    bool obscureText = false,
  }) => TextFormField(
    controller: controller,
    obscureText: obscureText,
    decoration: InputDecoration(
      labelText: text,
      border: const OutlineInputBorder(),
    ),
    validator: (value) {
      if (value == null ||
          value.trim().isEmpty ||
          (additionalValidator != null && additionalValidator(value))) {
        return "$text is invalid";
      }
      return null;
    },
  );

  Widget numberFormField({
    required TextEditingController controller,
    required String text,
  }) => TextFormField(
    controller: controller,
    readOnly: true,
    decoration: InputDecoration(
      labelText: text,
      border: const OutlineInputBorder(),
    ),
    validator: (value) {
      if (value == null ||
          value.trim().isEmpty) {
        return "$text is empty";
      }
      return null;
    },
  );

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
                      const SizedBox(height: 16),
                      Text(
                        'Birthday:',
                        style: TextStyle(
                          fontSize: 16,
                          color: Colors.grey.shade700,
                        ),
                      ),
                      Row(
                        children: [
                          Expanded(
                            child: numberFormField(
                              controller: _birthdayDayController,
                              text: 'Day',
                            ),
                          ),
                          SizedBox(width: 8),
                          Expanded(
                            child: numberFormField(
                              controller: _birthdayMonthController,
                              text: 'Month',
                            ),
                          ),
                          SizedBox(width: 8),
                          Expanded(
                            child: numberFormField(
                              controller: _birthdayYearController,
                              text: 'Year',
                            ),
                          ),
                          SizedBox(width: 8),
                          Flexible(
                            fit: FlexFit.tight,
                            child: ElevatedButton(
                              onPressed: () async => pickDate(context),
                              child: Text('Set Your Birthday'),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      TextFormField(
                        controller: _companyController,
                        decoration: InputDecoration(
                          labelText: 'Company Name',
                          border: const OutlineInputBorder(),
                        ),
                      ),
                      const SizedBox(height: 48),

                      // Account info
                      nonEmptyTextFormField(
                        controller: _loginController,
                        text: 'Login',
                      ),
                      const SizedBox(height: 16),
                      nonEmptyTextFormField(
                        controller: _emailController,
                        text: 'Email',
                        additionalValidator: (s) => !RegExp(
                          r"^[a-zA-Z0-9.a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9]+\.[a-zA-Z]+",
                        ).hasMatch(s),
                      ),
                      const SizedBox(height: 16),
                      nonEmptyTextFormField(
                        controller: _passwordController,
                        text: 'Password',
                        obscureText: true,
                        additionalValidator: (s) => s.length < 6,
                      ),
                      const SizedBox(height: 24),

                      // Button
                      ElevatedButton(
                        onPressed: _register,
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

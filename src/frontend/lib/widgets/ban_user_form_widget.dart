import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class BanUserFormWidget extends StatefulWidget {
  final void Function(int userId, String reason, DateTime expiresAt) onSubmit;

  const BanUserFormWidget({super.key, required this.onSubmit, this.userId});

  final int? userId;

  @override
  State<BanUserFormWidget> createState() => _BanUserFormWidgetState();
}

class _BanUserFormWidgetState extends State<BanUserFormWidget> {
  final _formKey = GlobalKey<FormState>();

  final _userIdController = TextEditingController();
  final _reasonController = TextEditingController();

  DateTime? _expiresAt;

  @override
  void initState() {
    super.initState();
    _userIdController.text = widget.userId.toString();
  }

  @override
  void dispose() {
    _userIdController.dispose();
    _reasonController.dispose();
    super.dispose();
  }

  Future<void> _pickDate() async {
    final selectedDate = await showDatePicker(
      context: context,
      initialDate: DateTime.now().add(const Duration(days: 1)),
      firstDate: DateTime.now(),
      lastDate: DateTime(2100),
    );

    if (selectedDate != null) {
      setState(() {
        _expiresAt = selectedDate;
      });
    }
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    if (_expiresAt == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select an expiration date.')),
      );
      return;
    }

    widget.onSubmit(
      int.parse(_userIdController.text),
      _reasonController.text.trim(),
      _expiresAt!,
    );
  }

  String _formatDate(DateTime date) {
    return "${date.day.toString().padLeft(2, '0')}-"
        "${date.month.toString().padLeft(2, '0')}-"
        "${date.year}";
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              "Ban User",
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),

            const SizedBox(height: 32),

            if (widget.userId == null) ...[
            Card(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
                side: BorderSide(color: Theme.of(context).primaryColor),
              ),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: TextFormField(
                  controller: _userIdController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    labelText: "User ID",
                    prefixIcon: Icon(Icons.person),
                    border: OutlineInputBorder(),
                  ),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Please enter a user ID';
                    }

                    final id = int.tryParse(value);

                    if (id == null || id <= 0) {
                      return 'Please enter a valid user ID';
                    }

                    return null;
                  },
                ),
              ),
            ),
            ],

            const SizedBox(height: 16),

            Card(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
                side: BorderSide(color: Theme.of(context).primaryColor),
              ),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: TextFormField(
                  controller: _reasonController,
                  maxLines: 4,
                  decoration: const InputDecoration(
                    labelText: "Reason",
                    prefixIcon: Icon(Icons.gavel),
                    border: OutlineInputBorder(),
                    alignLabelWithHint: true,
                  ),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Please provide a reason';
                    }

                    if (value.trim().length < 5) {
                      return 'Reason is too short';
                    }

                    return null;
                  },
                ),
              ),
            ),

            const SizedBox(height: 16),

            Card(
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(8),
                side: BorderSide(color: Theme.of(context).primaryColor),
              ),
              child: InkWell(
                borderRadius: BorderRadius.circular(8),
                onTap: _pickDate,
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Row(
                    children: [
                      Icon(
                        Icons.calendar_month,
                        color: Theme.of(context).primaryColor,
                      ),
                      const SizedBox(width: 16),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text(
                              "Ban Expiration Date",
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                            const SizedBox(height: 4),
                            Text(
                              _expiresAt == null
                                  ? "Select expiration date"
                                  : _formatDate(_expiresAt!),
                              style: TextStyle(
                                color: _expiresAt == null
                                    ? Colors.grey
                                    : Colors.black,
                              ),
                            ),
                          ],
                        ),
                      ),
                      const Icon(Icons.arrow_forward_ios, size: 16),
                    ],
                  ),
                ),
              ),
            ),

            if (_expiresAt == null)
              const Padding(
                padding: EdgeInsets.only(top: 8, left: 12),
                child: Text(
                  "Expiration date is required",
                  style: TextStyle(color: Colors.red, fontSize: 12),
                ),
              ),

            const SizedBox(height: 48),

            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: () {
                  _submit();
                  context.pop();
                },
                icon: const Icon(Icons.block),
                label: const Text("Ban User", style: TextStyle(fontSize: 18)),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.red,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

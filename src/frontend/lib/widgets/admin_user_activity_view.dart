import 'package:flutter/material.dart';
import 'package:frontend/api/api_admin.dart';
import 'package:frontend/models/activity.dart';
import 'package:intl/intl.dart';

class AdminUserActivityView extends StatefulWidget {
  const AdminUserActivityView({super.key});

  @override
  State<StatefulWidget> createState() => _AdminUserActivityViewState();
}

class _AdminUserActivityViewState extends State<AdminUserActivityView> {
  final apiAdmin = ApiAdmin();
  final _userIdController = TextEditingController();

  Future<List<Activity>>? _activityFuture;

  void _fetchActivity() {
    final userIdText = _userIdController.text.trim();
    if (userIdText.isEmpty) {
      return;
    }

    final userId = int.tryParse(userIdText);
    if (userId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a valid User ID')),
      );
      return;
    }

    setState(() {
      _activityFuture = apiAdmin.getUserActivity(userId);
    });
  }

  @override
  void dispose() {
    _userIdController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Row(
            children: [
              Expanded(
                child: TextFormField(
                  controller: _userIdController,
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    labelText: 'User ID',
                    border: OutlineInputBorder(),
                  ),
                  onFieldSubmitted: (_) => _fetchActivity(),
                ),
              ),
              const SizedBox(width: 16),
              FilledButton(
                onPressed: _fetchActivity,
                style: FilledButton.styleFrom(
                  padding: const EdgeInsets.symmetric(
                    vertical: 16,
                    horizontal: 24,
                  ),
                ),
                child: const Text('Search'),
              ),
            ],
          ),
        ),
        const Divider(height: 1),
        Expanded(
          child: _activityFuture == null
              ? const Center(child: Text('Enter a User ID to view activities'))
              : FutureBuilder<List<Activity>>(
                  future: _activityFuture,
                  builder: (context, snapshot) {
                    if (snapshot.connectionState == ConnectionState.waiting) {
                      return const Center(child: CircularProgressIndicator());
                    }
                    if (snapshot.hasError) {
                      return Center(
                        child: Text(
                          snapshot.error.toString().trim().replaceAll(
                            'Exception: ',
                            '',
                          ),
                        ),
                      );
                    }
                    if (snapshot.hasData && snapshot.data != null) {
                      final activities = snapshot.data!;
                      if (activities.isEmpty) {
                        return const Center(
                          child: Text('No activities found for this user.'),
                        );
                      }
                      return _buildActivityList(activities, theme);
                    }
                    return const SizedBox();
                  },
                ),
        ),
      ],
    );
  }

  Widget _buildActivityList(List<Activity> activities, ThemeData theme) {
    return ListView.builder(
      itemCount: activities.length,
      itemBuilder: (context, index) {
        final activity = activities[index];
        final dateFormatted = DateFormat(
          'yyyy-MM-dd HH:mm:ss',
        ).format(activity.createdAt);

        return Card(
          margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
          child: Padding(
            padding: const EdgeInsets.all(12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      activity.action,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    Text(dateFormatted, style: theme.textTheme.bodySmall),
                  ],
                ),
                const SizedBox(height: 8),
                Text('Activity ID: ${activity.id}'),
                if (activity.offerId != null)
                  Text('Offer ID: ${activity.offerId}'),
                Text('IP Address: ${activity.ipAddress}'),
                if (activity.details.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  const Text(
                    'Details:',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  ...activity.details.entries.map(
                    (entry) => Text('- ${entry.key}: ${entry.value}'),
                  ),
                ],
              ],
            ),
          ),
        );
      },
    );
  }
}

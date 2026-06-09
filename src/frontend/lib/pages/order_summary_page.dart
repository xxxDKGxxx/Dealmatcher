import 'package:flutter/material.dart';
import 'package:frontend/api/api_cart.dart';
import 'package:frontend/api/api_purchases.dart';
import 'package:frontend/models/cart_item.dart';
import 'package:frontend/models/delivery_method.dart';
import 'package:frontend/models/payment_method.dart';
import 'package:frontend/models/price.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';
import 'package:frontend/widgets/placeholder_image_widget.dart';
import 'package:url_launcher/url_launcher.dart';

class OrderSummaryPage extends StatefulWidget {
  const OrderSummaryPage({
    super.key,
    this.apiCart,
    required this.deliveryMethod,
    required this.paymentMethod,
  });

  final ApiCart? apiCart;
  final DeliveryMethod deliveryMethod;
  final PaymentMethod paymentMethod;

  @override
  State<OrderSummaryPage> createState() => _OrderSummaryPageState();
}

class _OrderSummaryPageState extends State<OrderSummaryPage> {
  late final ApiCart _apiCart = widget.apiCart ?? ApiCart();
  final ApiPurchases _apiPurchases = ApiPurchases();

  late Future<List<CartItem>> _cartItemsFuture;
  late Future<Price> _cartTotalFuture;

  @override
  void initState() {
    super.initState();
    _loadCartData();
  }

  void _loadCartData() {
    setState(() {
      _cartItemsFuture = _apiCart.getCart();
      _cartTotalFuture = _apiCart.getCartTotal();
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: FutureBuilder<List<CartItem>>(
        future: _cartItemsFuture,
        builder: (context, itemsSnapshot) {
          if (itemsSnapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (itemsSnapshot.hasError) {
            return _buildErrorState();
          }

          final items = itemsSnapshot.data ?? [];
          if (items.isEmpty) {
            return const Center(child: Text('Your cart is empty.'));
          }

          final delivery = widget.deliveryMethod;
          final payment = widget.paymentMethod;

          return Column(
            children: [
              Expanded(
                child: ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    Text(
                      'Order Summary',
                      style: theme.textTheme.displaySmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 24),
                    ...items.map((item) => _buildReadOnlyItem(item)),
                    const Divider(height: 40),
                    _buildSectionHeader(
                      Icons.local_shipping,
                      'Delivery Method',
                    ),
                    Padding(
                      padding: EdgeInsets.symmetric(vertical: 8.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Expanded(
                                child: Text(
                                  delivery.name,
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 16,
                                  ),
                                ),
                              ),
                              Text(
                                '\$${delivery.price.toStringAsFixed(2)}',
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 16,
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 4),
                          Text(delivery.description),
                          const SizedBox(height: 4),
                          Row(
                            children: [
                              const Icon(
                                Icons.access_time,
                                size: 16,
                                color: Colors.grey,
                              ),
                              const SizedBox(width: 4),
                              Text(
                                'Estimated: ${delivery.estimatedDays} day${delivery.estimatedDays > 1 ? 's' : ''}',
                                style: const TextStyle(
                                  color: Colors.grey,
                                  fontSize: 13,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 24),
                    _buildSectionHeader(Icons.payment, 'Payment Method'),
                    Padding(
                      padding: EdgeInsets.symmetric(vertical: 8.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            payment.name,
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 16,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'Provider: ${payment.provider}',
                            style: const TextStyle(
                              color: Colors.grey,
                              fontSize: 13,
                            ),
                          ),
                        ],
                      ),
                    ),

                    const SizedBox(height: 40),
                  ],
                ),
              ),
              _buildBottomSummaryPanel(items),
            ],
          );
        },
      ),
    );
  }

  Widget _buildReadOnlyItem(CartItem item) {
    final offer = item.offer;
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      elevation: 0,
      shape: RoundedRectangleBorder(
        side: BorderSide(color: Colors.grey.shade200),
        borderRadius: BorderRadius.circular(12),
      ),
      child: ListTile(
        contentPadding: const EdgeInsets.all(12),
        leading: ClipRRect(
          borderRadius: BorderRadius.circular(8),
          child: SizedBox(
            width: 60,
            height: 60,
            child: offer.images.isNotEmpty
                ? Image.network(offer.images.first, fit: BoxFit.cover)
                : placeholderImageWidget(),
          ),
        ),
        title: Text(
          offer.title,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Text('Quantity: ${item.quantity}'),
        trailing: Text(
          '${offer.price} zł',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
      ),
    );
  }

  Widget _buildSectionHeader(IconData icon, String title) {
    return Row(
      children: [
        Icon(icon, size: 20, color: Colors.grey[700]),
        const SizedBox(width: 8),
        Text(
          title,
          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
      ],
    );
  }

  Widget _buildBottomSummaryPanel(List<CartItem> items) {
    return FutureBuilder<Price>(
      future: _cartTotalFuture,
      builder: (context, totalSnapshot) {
        String totalText = '...';
        if (totalSnapshot.hasData) {
          final price = totalSnapshot.data!;
          totalText = '${price.value} ${price.currency}';
        }

        return Container(
          padding: const EdgeInsets.all(24),
          decoration: BoxDecoration(
            color: Theme.of(context).cardColor,
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, -5),
              ),
            ],
          ),
          child: SafeArea(
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Text(
                      'Total to pay:',
                      style: TextStyle(color: Colors.grey),
                    ),
                    Text(
                      totalText,
                      style: const TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
                ElevatedButton(
                  onPressed: !totalSnapshot.hasData
                      ? null
                      : () async {
                          for (var item in items) {
                            final redirectUrl = await _apiPurchases
                                .initializePurchase(
                                  item.offer.id,
                                  widget.deliveryMethod.id,
                                  widget.paymentMethod.id,
                                  item.quantity,
                                );
                            final Uri url = Uri.parse(redirectUrl);
                            if (!await launchUrl(url)) {
                              throw Exception('Could not launch $redirectUrl');
                            }
                          }
                        },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.green[700],
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 32,
                      vertical: 16,
                    ),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                  child: const Text(
                    'Place Order',
                    style: TextStyle(fontSize: 16),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  Widget _buildErrorState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const Icon(Icons.error_outline, size: 48, color: Colors.red),
          const SizedBox(height: 16),
          const Text('Something went wrong'),
          ElevatedButton(
            onPressed: _loadCartData,
            child: const Text('Try Again'),
          ),
        ],
      ),
    );
  }
}

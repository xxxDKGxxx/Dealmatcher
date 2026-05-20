import 'package:flutter/material.dart';
import 'package:frontend/api/api_cart.dart';
import 'package:frontend/models/cart_item.dart';
import 'package:frontend/models/price.dart';
import 'package:frontend/widgets/dealmatcher_app_bar.dart';

class CartPage extends StatefulWidget {
  const CartPage({super.key});

  @override
  State<CartPage> createState() => _CartPageState();
}

class _CartPageState extends State<CartPage> {
  final ApiCart _apiCart = ApiCart();

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

  void _showErrorSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
      ),
    );
  }

  Future<void> _updateQuantity(int itemId, int newQuantity) async {
    if (newQuantity < 1) return;
    try {
      await _apiCart.updateItemQuantity(itemId, newQuantity);
      _loadCartData();
    } catch (e) {
      _showErrorSnackBar('Could not update quantity: ${e.toString()}');
    }
  }

  Future<void> _removeItem(int itemId) async {
    try {
      await _apiCart.removeItem(itemId);
      _loadCartData();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Removed item from cart.')),
      );
    } catch (e) {
      _showErrorSnackBar('Could not remove item from cart: ${e.toString()}');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: DealmatcherAppBar(),
      body: FutureBuilder<List<CartItem>>(
        future: _cartItemsFuture,
        builder: (context, itemsSnapshot) {
          if (itemsSnapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (itemsSnapshot.hasError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.error_outline, size: 48, color: Colors.red),
                  const SizedBox(height: 16),
                  Text('Error: ${itemsSnapshot.error}'),
                  ElevatedButton(
                    onPressed: _loadCartData,
                    child: const Text('Try Again'),
                  ),
                ],
              ),
            );
          }

          final items = itemsSnapshot.data ?? [];

          if (items.isEmpty) {
            return const Center(
              child: Text(
                'Your cart is empty.',
                style: TextStyle(fontSize: 18, color: Colors.grey),
              ),
            );
          }

          return Column(
            children: [
              Expanded(
                child: ListView.builder(
                  itemCount: items.length,
                  itemBuilder: (context, index) {
                    final item = items[index];
                    return Card(
                      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                      child: ListTile(
                        leading: const CircleAvatar(
                          child: Icon(Icons.shopping_bag),
                        ),
                        title: Text('Item #${item.id}'),
                        subtitle: Row(
                          children: [
                            IconButton(
                              icon: const Icon(Icons.remove_circle_outline),
                              onPressed: () => _updateQuantity(item.id, item.quantity - 1),
                            ),
                            Text(
                              '${item.quantity}',
                              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                            ),
                            IconButton(
                              icon: const Icon(Icons.add_circle_outline),
                              onPressed: () => _updateQuantity(item.id, item.quantity + 1),
                            ),
                          ],
                        ),
                        trailing: IconButton(
                          icon: const Icon(Icons.delete_outline, color: Colors.red),
                          onPressed: () => _removeItem(item.id),
                        ),
                      ),
                    );
                  },
                ),
              ),

              _buildSummaryPanel(),
            ],
          );
        },
      ),
    );
  }

  Widget _buildSummaryPanel() {
    return FutureBuilder<Price>(
      future: _cartTotalFuture,
      builder: (context, totalSnapshot) {
        String totalText = 'Loading...';

        if (totalSnapshot.hasData && totalSnapshot.connectionState != ConnectionState.waiting) {
          final price = totalSnapshot.data!;
          totalText = '${price.value} ${price.currency}';
        } else if (totalSnapshot.hasError) {
          totalText = 'Error loading total.';
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
                      'Total:',
                      style: TextStyle(color: Colors.grey, fontSize: 14),
                    ),
                    Text(
                      totalText,
                      style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
                ElevatedButton(
                  onPressed: totalSnapshot.hasData ? () {
                  } : null,
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 16),
                  ),
                  child: const Text('Payment'),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
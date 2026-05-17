import 'package:flutter/material.dart';
import 'package:frontend/api/api_purchases.dart';
import 'package:frontend/models/delivery_method.dart';
import 'package:frontend/models/payment_method.dart';

class CheckoutFormWidget extends StatefulWidget {
  final void Function(
    DeliveryMethod deliveryMethod,
    PaymentMethod paymentMethod,
  )
  onSubmit;

  const CheckoutFormWidget({super.key, required this.onSubmit});

  @override
  State<CheckoutFormWidget> createState() => _CheckoutFormWidgetState();
}

class _CheckoutFormWidgetState extends State<CheckoutFormWidget> {
  final _formKey = GlobalKey<FormState>();

  DeliveryMethod? _selectedDelivery;
  PaymentMethod? _selectedPayment;

  late Future<List<DeliveryMethod>> _deliveryMethodsFuture;
  late Future<List<PaymentMethod>> _paymentMethodsFuture;

  @override
  void initState() {
    super.initState();
    _deliveryMethodsFuture = ApiPurchases().getDeliveryMethods();
    _paymentMethodsFuture = ApiPurchases().getPaymentMethods();
  }

  void _submit() {
    if (_formKey.currentState!.validate()) {
      if (_selectedDelivery == null || _selectedPayment == null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Please select delivery and payment methods.'),
          ),
        );
        return;
      }

      widget.onSubmit(_selectedDelivery!, _selectedPayment!);
    }
  }

  Widget _buildDeliveryList(
    List<DeliveryMethod> methods,
    FormFieldState<DeliveryMethod> state,
  ) {
    return Column(
      children: methods.map((method) {
        final isSelected = _selectedDelivery?.id == method.id;
        return Card(
          elevation: isSelected ? 2 : 0,
          shape: RoundedRectangleBorder(
            side: BorderSide(
              color: !isSelected
                  ? Theme.of(context).primaryColor
                  : Colors.grey.shade300,
              width: isSelected ? 2 : 1,
            ),
            borderRadius: BorderRadius.circular(8),
          ),
          margin: const EdgeInsets.only(bottom: 8),
          child: InkWell(
            borderRadius: BorderRadius.circular(8),
            onTap: () {
              setState(() {
                _selectedDelivery = method;
              });
              state.didChange(method);
            },
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  Icon(
                    isSelected
                        ? Icons.radio_button_checked
                        : Icons.radio_button_unchecked,
                    color: !isSelected
                        ? Theme.of(context).primaryColor
                        : Colors.grey,
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Expanded(
                              child: Text(
                                method.name,
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 16,
                                ),
                              ),
                            ),
                            Text(
                              '\$${method.price.toStringAsFixed(2)}',
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 4),
                        Text(method.description),
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
                              'Estimated: ${method.estimatedDays} day${method.estimatedDays > 1 ? 's' : ''}',
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
                ],
              ),
            ),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildPaymentList(
    List<PaymentMethod> methods,
    FormFieldState<PaymentMethod> state,
  ) {
    return Column(
      children: methods.map((method) {
        final isSelected = _selectedPayment?.id == method.id;
        return Card(
          elevation: isSelected ? 2 : 0,
          shape: RoundedRectangleBorder(
            side: BorderSide(
              color: !isSelected
                  ? Theme.of(context).primaryColor
                  : Colors.grey.shade300,
              width: isSelected ? 2 : 1,
            ),
            borderRadius: BorderRadius.circular(8),
          ),
          margin: const EdgeInsets.only(bottom: 8),
          child: InkWell(
            borderRadius: BorderRadius.circular(8),
            onTap: () {
              setState(() {
                _selectedPayment = method;
              });
              state.didChange(method);
            },
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Row(
                children: [
                  Icon(
                    isSelected
                        ? Icons.radio_button_checked
                        : Icons.radio_button_unchecked,
                    color: !isSelected
                        ? Theme.of(context).primaryColor
                        : Colors.grey,
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          method.name,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 16,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Provider: ${method.provider}',
                          style: const TextStyle(
                            color: Colors.grey,
                            fontSize: 13,
                          ),
                        ),
                      ],
                    ),
                  ),
                  SizedBox(
                    width: 40,
                    height: 40,
                    child: Image.network(
                      method.icon,
                      errorBuilder: (context, error, stackTrace) {
                        return const Icon(
                          Icons.payment,
                          size: 40,
                          color: Colors.grey,
                        );
                      },
                      fit: BoxFit.contain,
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      }).toList(),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Contact and Shipping Details",
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 32),
          const Text(
            "Delivery Method",
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          FormField<DeliveryMethod>(
            validator: (value) {
              if (_selectedDelivery == null) {
                return 'Please select a delivery method';
              }
              return null;
            },
            builder: (FormFieldState<DeliveryMethod> state) {
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  FutureBuilder<List<DeliveryMethod>>(
                    future: _deliveryMethodsFuture,
                    builder: (context, snapshot) {
                      if (snapshot.connectionState == ConnectionState.waiting) {
                        return const Center(child: CircularProgressIndicator());
                      } else if (snapshot.hasError) {
                        return Text(
                          'Error loading delivery methods: ${snapshot.error}',
                        );
                      } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                        return const Text('No delivery methods available');
                      }

                      final methods = snapshot.data!;
                      if (_selectedDelivery != null &&
                          !methods.any((m) => m.id == _selectedDelivery!.id)) {
                        _selectedDelivery = null;
                      }

                      return _buildDeliveryList(methods, state);
                    },
                  ),
                  if (state.hasError)
                    Padding(
                      padding: const EdgeInsets.only(top: 8, left: 12),
                      child: Text(
                        state.errorText!,
                        style: TextStyle(
                          color: Theme.of(context).colorScheme.error,
                          fontSize: 12,
                        ),
                      ),
                    ),
                ],
              );
            },
          ),
          const SizedBox(height: 32),
          const Text(
            "Payment Method",
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          FormField<PaymentMethod>(
            validator: (value) {
              if (_selectedPayment == null) {
                return 'Please select a payment method';
              }
              return null;
            },
            builder: (FormFieldState<PaymentMethod> state) {
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  FutureBuilder<List<PaymentMethod>>(
                    future: _paymentMethodsFuture,
                    builder: (context, snapshot) {
                      if (snapshot.connectionState == ConnectionState.waiting) {
                        return const Center(child: CircularProgressIndicator());
                      } else if (snapshot.hasError) {
                        return Text(
                          'Error loading payment methods: ${snapshot.error}',
                        );
                      } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
                        return const Text('No payment methods available');
                      }

                      final methods = snapshot.data!;
                      if (_selectedPayment != null &&
                          !methods.any((m) => m.id == _selectedPayment!.id)) {
                        _selectedPayment = null;
                      }

                      return _buildPaymentList(methods, state);
                    },
                  ),
                  if (state.hasError)
                    Padding(
                      padding: const EdgeInsets.only(top: 8, left: 12),
                      child: Text(
                        state.errorText!,
                        style: TextStyle(
                          color: Theme.of(context).colorScheme.error,
                          fontSize: 12,
                        ),
                      ),
                    ),
                ],
              );
            },
          ),
          const SizedBox(height: 48),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: _submit,
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: const Text("Place Order", style: TextStyle(fontSize: 18)),
            ),
          ),
        ],
      ),
    );
  }
}

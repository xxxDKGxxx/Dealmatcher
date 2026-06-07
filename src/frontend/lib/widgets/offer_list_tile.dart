import 'package:flutter/material.dart';
import 'package:frontend/models/offer.dart';
import 'package:frontend/widgets/placeholder_image_widget.dart';

Widget offerListTile({
  required Offer offer,
  required ThemeData theme,
  required void Function() onDelete,
  required void Function() activateOffer,
}) {
  return Card(
    child: SizedBox(
      height: 120,
      child: Row(
        children: [
          AspectRatio(
            aspectRatio: 1,
            child: Padding(
              padding: const EdgeInsets.all(8),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: offer.images.isNotEmpty
                    ? Image.network(offer.images.first, fit: BoxFit.cover)
                    : placeholderImageWidget(),
              ),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    offer.title,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Category: ${offer.category.name}',
                    style: theme.textTheme.bodyMedium,
                  ),
                  Text(
                    'Price: ${offer.price.toStringAsFixed(2)}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Seller: ${offer.seller.name}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
          ),
          if (offer.status == OfferStatus.draft) ...[
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 12),
              child: TextButton.icon(
                onPressed: activateOffer,
                icon: Icon(Icons.check_rounded),
                label: Text('Activate'),
              ),
            ),
          ],
          Padding(
            padding: EdgeInsets.symmetric(horizontal: 12),
            child: IconButton(
              onPressed: onDelete,
              icon: Icon(Icons.delete_rounded),
            ),
          ),
        ],
      ),
    ),
  );
}

import 'package:frontend/api/models/request_model.dart';

class OfferDetailsRequest extends RequestModel {
  const OfferDetailsRequest({required this.offerId});

  final int offerId;

  @override
  String toJson() {
    return offerId.toString();
  }
}

import 'package:http/http.dart';

abstract class ResponseModel {
  const ResponseModel({required this.response});
  final Response response;
  void fromJson();
}

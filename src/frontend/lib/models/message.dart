class Message {
  const Message({
    required this.id,
    required this.senderId,
    required this.content,
    required this.status,
    required this.createdAt,
  });

  final int id;
  final int senderId;
  final String content;
  final String status;
  final DateTime createdAt;

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      id: json['id'] as int,
      senderId: json['senderId'] as int,
      content: json['content'] as String,
      status: json['status'] as String,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'senderId': senderId,
      'content': content,
      'status': status,
      'createdAt': createdAt.toIso8601String(),
    };
  }
}

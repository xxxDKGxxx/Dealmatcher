namespace Dealmatcher.Backend.API.Endpoints.Conversations.Create;

public sealed record CreateRequest(
    int OfferId,
    string InitialMessage);

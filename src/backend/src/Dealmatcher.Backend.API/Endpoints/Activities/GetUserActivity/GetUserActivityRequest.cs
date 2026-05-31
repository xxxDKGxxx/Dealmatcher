namespace Dealmatcher.Backend.API.Endpoints.Activities.GetUserActivity;

public sealed record GetUserActivityRequest(int UserId, DateTime From, DateTime To);

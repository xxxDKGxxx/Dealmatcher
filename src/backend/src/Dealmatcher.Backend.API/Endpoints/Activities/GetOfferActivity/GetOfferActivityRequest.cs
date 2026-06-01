namespace Dealmatcher.Backend.API.Endpoints.Activities.GetOfferActivity;

public sealed record GetOfferActivityRequest(int OfferId, DateTime From, DateTime To);

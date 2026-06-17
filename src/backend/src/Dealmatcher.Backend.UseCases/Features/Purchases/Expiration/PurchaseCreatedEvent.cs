namespace Dealmatcher.Backend.UseCases.Features.Purchases.Expiration;

public sealed record PurchaseCreatedEvent(int PurchaseId) : INotification;

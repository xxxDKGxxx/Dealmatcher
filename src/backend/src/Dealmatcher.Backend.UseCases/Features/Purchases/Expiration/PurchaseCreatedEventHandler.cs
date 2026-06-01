namespace Dealmatcher.Backend.UseCases.Features.Purchases.Expiration;

public sealed class PurchaseCreatedEventHandler(IPurchaseExpirationQueue expirationQueue) : INotificationHandler<PurchaseCreatedEvent>
{
    public async Task Handle(PurchaseCreatedEvent notification, CancellationToken cancellationToken)
    {
        await expirationQueue.EnqueueAsync(notification.PurchaseId, cancellationToken);
    }
}

namespace Dealmatcher.Backend.UseCases.Interfaces;

public interface IPurchaseExpirationQueue
{
    ValueTask EnqueueAsync(int purchaseId, CancellationToken cancellationToken);
}

using Microsoft.Extensions.Hosting;

namespace Dealmatcher.Backend.Infrastructure.Services.Purchases;

public sealed class PurchaseExpirationService(
    PurchaseExpirationQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<PurchaseExpirationService> logger) : BackgroundService
{
    private static readonly TimeSpan _purchaseTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan _retryDelay = TimeSpan.FromMilliseconds(200);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("PurchaseExpirationService started.");

        await foreach (var purchaseId in queue.ReadAllAsync(ct))
        {
            _ = ExpireAfterTtl(purchaseId, ct);
        }
    }

    private async Task ExpireAfterTtl(int purchaseId, CancellationToken ct)
    {
        try
        {
            await Task.Delay(_purchaseTtl, ct);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                if (await TryExpireOnce(purchaseId, ct))
                {
                    logger.LogInformation("Purchase {PurchaseId} expired after TTL.", purchaseId);
                }

                return;
            }
            catch (ConcurrencyException)
            {
                logger.LogWarning("Concurrency conflict expiring purchase {PurchaseId}; retrying with fresh state.", purchaseId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error expiring purchase {PurchaseId}.", purchaseId);
                return;
            }

            try
            {
                await Task.Delay(_retryDelay, ct);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }

    private async Task<bool> TryExpireOnce(int purchaseId, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var purchaseRepository = scope.ServiceProvider.GetRequiredService<IRepository<Purchase>>();
        var offerRepository = scope.ServiceProvider.GetRequiredService<IRepository<Offer>>();

        var purchase = await purchaseRepository.GetByIdAsync(purchaseId, ct);
        if (purchase is null || purchase.Status != PurchaseStatus.Pending)
            return false;

        purchase.Fail();

        var offer = await offerRepository.GetByIdAsync(purchase.Offer.Id, ct);
        offer?.RestoreQuantity(purchase.Quantity);

        await purchaseRepository.SaveChangesAsync(ct);
        return true;
    }
}

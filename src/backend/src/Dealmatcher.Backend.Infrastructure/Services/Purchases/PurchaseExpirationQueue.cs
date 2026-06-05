using System.Threading.Channels;

namespace Dealmatcher.Backend.Infrastructure.Services.Purchases;

public sealed class PurchaseExpirationQueue : IPurchaseExpirationQueue
{
    private readonly Channel<int> _channel = Channel.CreateUnbounded<int>();

    public ValueTask EnqueueAsync(int purchaseId, CancellationToken ct) =>
        _channel.Writer.WriteAsync(purchaseId, ct);

    public IAsyncEnumerable<int> ReadAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}

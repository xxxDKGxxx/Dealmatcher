using Dealmatcher.Backend.Domain.Interfaces.Delivery;

namespace Dealmatcher.Backend.Infrastructure.Services.DeliveryProviders;

public sealed class ExampleDeliveryProvider : IDeliveryProvider
{
    public string Name => "ExampleCourier";

    public Task<string> RegisterParcelAsync(int orderId, string targetAddress)
    {
        return Task.FromResult($"MOCK-TRACKING-{orderId}");
    }
}

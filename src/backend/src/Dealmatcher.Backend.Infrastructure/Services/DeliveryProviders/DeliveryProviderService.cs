using Dealmatcher.Backend.Domain.Interfaces.Delivery;

namespace Dealmatcher.Backend.Infrastructure.Services.DeliveryProviders;

public sealed class DeliveryProviderService(IEnumerable<IDeliveryProvider> providers) : IDeliveryProviderService
{
    private readonly Dictionary<string, IDeliveryProvider> _providers = providers.ToDictionary(p => p.Name, p => p);

    public IDeliveryProvider GetDeliveryProviderByName(string providerName)
    {
        return _providers.TryGetValue(providerName, out var provider)
            ? provider
            : throw new ArgumentException($"Wrong DeliveryProviderName: {providerName}");
    }
}

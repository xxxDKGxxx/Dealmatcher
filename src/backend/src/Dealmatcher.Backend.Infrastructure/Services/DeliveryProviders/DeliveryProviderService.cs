namespace Dealmatcher.Backend.Infrastructure.Services.DeliveryProviders;

public sealed class DeliveryProviderService(IEnumerable<IDeliveryProvider> providers) : IDeliveryProviderService
{
    private readonly Dictionary<string, IDeliveryProvider> _providers = providers.ToDictionary(p => p.Id, p => p);

    public IReadOnlyCollection<IDeliveryProvider> GetAllDeliveryProviders()
    {
        return _providers.Values.ToList().AsReadOnly();
    }

    public IDeliveryProvider GetDeliveryProviderById(string id)
    {
        return _providers.TryGetValue(id, out var provider)
            ? provider
            : throw new ArgumentException($"Wrong DeliveryProviderId: {id}");
    }
}

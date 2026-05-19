namespace Dealmatcher.Backend.Infrastructure.Services.PaymentProviders;

public sealed class PaymentProviderService(IEnumerable<IPaymentProvider> providers) : IPaymentProviderService
{
    private readonly Dictionary<string, IPaymentProvider> _providers = providers.ToDictionary(p => p.Id, p => p);

    public IPaymentProvider GetPaymentProviderById(string providerId)
    {
        return _providers.TryGetValue(providerId, out var provider) ? provider : throw new ArgumentException($"Wrong PaymentProviderId: {providerId}");
    }
}

using Dealmatcher.Backend.Domain.Interfaces.Payment;

namespace Dealmatcher.Backend.Infrastructure.Services.PaymentProviders;
public sealed class PaymentProviderService(IEnumerable<IPaymentProvider> providers) : IPaymentProviderService
{
    private readonly Dictionary<string, IPaymentProvider> _providers = providers.ToDictionary(p => p.Name, p => p);

    public IPaymentProvider GetPaymentProviderByName(string providerName)
    {
        return _providers.TryGetValue(providerName, out var provider) ? provider : throw new ArgumentException($"Wrong PaymentProviderName: {providerName}");
    }
}

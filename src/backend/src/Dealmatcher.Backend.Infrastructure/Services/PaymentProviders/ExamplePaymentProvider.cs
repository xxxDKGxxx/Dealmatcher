using Dealmatcher.Backend.Domain.Interfaces.Payment;

namespace Dealmatcher.Backend.Infrastructure.Services.PaymentProviders;
public sealed class ExamplePaymentProvider : IPaymentProvider
{
    public string Name => "ExampleProviderName";

    public Task<string> GetPaymentRedirectUrl(decimal amount, string currency)
    {
        return Task.FromResult("exampleUrl");
    }
}

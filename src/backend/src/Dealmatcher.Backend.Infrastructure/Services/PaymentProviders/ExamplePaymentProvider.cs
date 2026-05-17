namespace Dealmatcher.Backend.Infrastructure.Services.PaymentProviders;

public sealed class ExamplePaymentProvider : IPaymentProvider
{
    public string Name => "ExampleProviderName";

    public Task<PaymentSession> CreatePaymentSessionAsync(decimal amount, string currency)
    {
        return Task.FromResult(new PaymentSession(Name, "exampleId", "exampleUrl", amount, currency));
    }

    public Task<PaymentStatus> GetPaymentStatusAsync(PaymentSession session)
    {
        return Task.FromResult(PaymentStatus.Pending);
    }
}

namespace Dealmatcher.Backend.Infrastructure.Services.PaymentProviders;

public sealed class ExamplePaymentProvider : IPaymentProvider
{
    public string Name => "ExampleProviderName";

    public string Id => "ExampleProviderId";

    public string Provider => "ExampleProvider";

    public string Icon => "ExampeIcon";

    public Task<PaymentSession> CreatePaymentSessionAsync(Purchase purchase)
    {
        return Task.FromResult(new PaymentSession(Name, "exampleId", "https://amber-gold-legit-payment-confirmation.netlify.app", purchase.TotalPrice, "PLN"));
    }

    public Task<PaymentStatus> GetPaymentStatusAsync(PaymentSession session)
    {
        return Task.FromResult(PaymentStatus.Pending);
    }

    public PaymentStatus? ParseStatus(string providerStatus)
    {
        try
        {
            return PaymentStatus.FromName(providerStatus, true);
        }
        catch
        {
            return null;
        }
    }
}

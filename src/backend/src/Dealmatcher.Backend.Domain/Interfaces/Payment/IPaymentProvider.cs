namespace Dealmatcher.Backend.Domain.Interfaces.Payment;

public interface IPaymentProvider
{
    public string Id { get; }
    public string Name { get; }
    public string Provider { get; }
    public string Icon { get; }
    public Task<PaymentSession> CreatePaymentSessionAsync(Purchase purchase);
    public Task<PaymentStatus> GetPaymentStatusAsync(PaymentSession session);
    public PaymentStatus? ParseStatus(string rawBody);
}

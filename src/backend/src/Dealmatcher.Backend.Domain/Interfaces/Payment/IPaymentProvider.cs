namespace Dealmatcher.Backend.Domain.Interfaces.Payment;

public interface IPaymentProvider
{
    public string Name { get; }
    public Task<PaymentSession> CreatePaymentSessionAsync(decimal amount, string currency);
    public Task<PaymentStatus> GetPaymentStatusAsync(PaymentSession session);
}

namespace Dealmatcher.Backend.Domain.Interfaces.Payment;

public interface IPaymentProvider
{
    public string Name { get; }
    public Task<string> GetPaymentRedirectUrl(decimal amount, string currency);
}

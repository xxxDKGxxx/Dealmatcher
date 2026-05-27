namespace Dealmatcher.Backend.Domain.Interfaces.Payment;

public interface IPaymentProviderService
{
    IPaymentProvider GetPaymentProviderById(string providerId);
    IReadOnlyCollection<IPaymentProvider> GetAllPaymentProviders();
}

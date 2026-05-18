namespace Dealmatcher.Backend.Domain.Interfaces.Payment;

public interface IPaymentProviderService
{
    IPaymentProvider GetPaymentProviderByName(string providerName);
}

namespace Dealmatcher.Backend.Domain.Interfaces.Delivery;

public interface IDeliveryProviderService
{
    IDeliveryProvider GetDeliveryProviderByName(string providerName);
}

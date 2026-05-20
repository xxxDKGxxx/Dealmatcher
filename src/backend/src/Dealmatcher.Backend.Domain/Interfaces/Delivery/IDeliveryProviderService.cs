namespace Dealmatcher.Backend.Domain.Interfaces.Delivery;

public interface IDeliveryProviderService
{
    IReadOnlyCollection<IDeliveryProvider> GetAllDeliveryProviders();
    IDeliveryProvider GetDeliveryProviderById(string id);
}

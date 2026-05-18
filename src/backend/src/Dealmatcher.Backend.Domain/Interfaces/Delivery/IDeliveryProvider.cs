namespace Dealmatcher.Backend.Domain.Interfaces.Delivery;

public interface IDeliveryProvider
{
    string Name { get; }

    Task<string> RegisterParcelAsync(DeliveryContext context);

    Task<int> GetEstimatedDaysAsync(DeliveryContext context);
}

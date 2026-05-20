namespace Dealmatcher.Backend.Domain.Interfaces.Delivery;

public interface IDeliveryProvider
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    decimal Price { get; }

    Task<string> RegisterParcelAsync(DeliveryContext context);
    Task<int> GetEstimatedDaysAsync(DeliveryContext context);
}

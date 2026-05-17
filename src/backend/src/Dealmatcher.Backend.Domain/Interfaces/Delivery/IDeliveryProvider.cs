namespace Dealmatcher.Backend.Domain.Interfaces.Delivery;

public interface IDeliveryProvider
{
    string Name { get; }

    Task<string> RegisterParcelAsync(int orderId, string targetAddress);
}

using Dealmatcher.Backend.Domain.Interfaces.Delivery;

namespace Dealmatcher.Backend.Infrastructure.Services.DeliveryProviders;

public sealed class ExampleDeliveryProvider : IDeliveryProvider
{
    public string Name => "ExampleCourier";

    public Task<string> RegisterParcelAsync(DeliveryContext context)
    {
        return Task.FromResult($"MOCK-TRACKING-{context.OfferId}");
    }

    public Task<int> GetEstimatedDaysAsync(DeliveryContext context)
    {
        int estimatedDays = context.RequestTime.DayOfWeek switch
        {
            DayOfWeek.Friday => 3,
            DayOfWeek.Saturday => 2,
            DayOfWeek.Sunday => 1,
            _ => 1
        };

        return Task.FromResult(estimatedDays);
    }
}

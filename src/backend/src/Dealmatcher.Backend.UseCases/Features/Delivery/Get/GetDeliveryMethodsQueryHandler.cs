namespace Dealmatcher.Backend.UseCases.Features.Delivery.Get;

public class GetDeliveryMethodsQueryHandler(
    IDeliveryProviderService deliveryProviderService,
    IMapper mapper) : IQueryHandler<GetDeliveryMethodsQuery, Result<List<DeliveryMethodDto>>>
{
    public async Task<Result<List<DeliveryMethodDto>>> Handle(GetDeliveryMethodsQuery request, CancellationToken cancellationToken)
    {
        // Nie mamy za bardzo sposobu na przekaznie kontekstu
        var context = new DeliveryContext(
            Buyer: null!,
            Seller: null!,
            OfferId: 0,
            RequestTime: DateTime.UtcNow
        );

        var providers = deliveryProviderService.GetAllDeliveryProviders();
        var dtos = new List<DeliveryMethodDto>();

        foreach (var provider in providers)
        {
            var estimatedDays = await provider.GetEstimatedDaysAsync(context);

            var dto = mapper.Map<DeliveryMethodDto>(provider);

            dtos.Add(dto with { EstimatedDays = estimatedDays });
        }

        return Result.Success(dtos);
    }
}

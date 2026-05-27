namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class DeliveryMethodProfile : Profile
{
    public DeliveryMethodProfile()
    {
        CreateMap<IDeliveryProvider, DeliveryMethodDto>()
            .ForCtorParam("EstimatedDays", opt => opt.MapFrom(src => 0));
    }
}

namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class PaymentMethodProfile : Profile
{
    public PaymentMethodProfile()
    {
        CreateMap<IPaymentProvider, PaymentMethodDto>();
    }
}

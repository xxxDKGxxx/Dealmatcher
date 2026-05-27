namespace Dealmatcher.Backend.UseCases.Features.Purchases.GetPaymentMethods;

public sealed class GetPaymentMethodsQueryHandler(
    IPaymentProviderService paymentProviderService,
    IMapper mapper) : IQueryHandler<GetPaymentMethodsQuery, Result<List<PaymentMethodDto>>>
{
    public Task<Result<List<PaymentMethodDto>>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success(paymentProviderService.GetAllPaymentProviders().Select(pp => mapper.Map<PaymentMethodDto>(pp)).ToList()));
    }
}

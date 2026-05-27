namespace Dealmatcher.Backend.UseCases.Features.Purchases.GetPaymentMethods;

public sealed record GetPaymentMethodsQuery : IQuery<Result<List<PaymentMethodDto>>>;

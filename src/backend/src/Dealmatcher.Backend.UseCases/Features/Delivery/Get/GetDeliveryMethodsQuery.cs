namespace Dealmatcher.Backend.UseCases.Features.Delivery.Get;

public sealed record GetDeliveryMethodsQuery() : IQuery<Result<List<DeliveryMethodDto>>>;

namespace Dealmatcher.Backend.UseCases.Features.Offers.Get;

public record GetOfferQuery(int OfferId) : IQuery<Result<OfferDto>>;

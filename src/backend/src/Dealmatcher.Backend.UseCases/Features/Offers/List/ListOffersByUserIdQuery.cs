namespace Dealmatcher.Backend.UseCases.Features.Offers.List;

public sealed record ListOffersByUserIdQuery(int UserId) : IQuery<Result<List<OfferDto>>>;

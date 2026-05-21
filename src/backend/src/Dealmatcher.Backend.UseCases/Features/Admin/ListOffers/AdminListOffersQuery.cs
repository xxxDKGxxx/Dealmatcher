namespace Dealmatcher.Backend.UseCases.Features.Admin.ListOffers;

public sealed record AdminListOffersQuery(
    int AdminId,
    int Page,
    int Limit,
    string Status) : IQuery<Result<AdminOffersPageDto>>;

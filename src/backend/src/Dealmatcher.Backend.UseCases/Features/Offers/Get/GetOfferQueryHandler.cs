namespace Dealmatcher.Backend.UseCases.Features.Offers.Get;

public class GetOfferQueryHandler(
    IReadRepository<Offer> offerRepository,
    IMapper mapper)
    : IQueryHandler<GetOfferQuery, Result<OfferDto>>
{
    public async Task<Result<OfferDto>> Handle(GetOfferQuery request, CancellationToken cancellationToken)
    {
        var spec = new OfferByIdWithDetailsSpec(request.OfferId);
        var offer = await offerRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (offer is null)
        {
            return Result.NotFound();
        }

        if (offer.Status == OfferStatus.Deleted)
        {
            return Result.NotFound();
        }

        var offerDto = mapper.Map<OfferDto>(offer);

        return Result.Success(offerDto);
    }
}

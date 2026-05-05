namespace Dealmatcher.Backend.UseCases.Features.Cart.Get;

public sealed class GetCartItemsQueryHandler(
    ICartRepository cartRepository,
    IReadRepository<Offer> offersRepository,
    IMapper mapper) : IQueryHandler<GetCartItemsQuery, Result<List<CartItemDto>>>
{
    public async Task<Result<List<CartItemDto>>> Handle(GetCartItemsQuery request, CancellationToken cancellationToken)
    {
        var cart = await cartRepository.GetCartAsync(request.UserId, cancellationToken);

        if (cart is null)
        {
            return Result.Error($"Encountered an error while retrieving user {request.UserId}'s cart");
        }

        var offersByIdsSpec = new OffersByIdsSpec(cart.Items.Select(i => i.OfferId));
        var offers = await offersRepository.ListAsync(offersByIdsSpec, cancellationToken);

        if (offers is null)
        {
            return Result.Error($"Couldn't retrieve offers");
        }

        var offerDtos = offers.Select(o => mapper.Map<OfferDto>(o)).ToList();
        var itemsWithOffers = cart.Items.Join(offerDtos, i => i.OfferId, o => o.Id, (i, o) => (i, o)).ToList();

        return Result.Success(itemsWithOffers.Select(t => mapper.Map<CartItemDto>(t)).ToList());
    }
}

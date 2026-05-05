namespace Dealmatcher.Backend.UseCases.Features.Cart.GetTotal;

public sealed class GetCartTotalQueryHandler(
    ICartRepository cartRepository,
    IReadRepository<Offer> offersRepository,
    IReadRepository<User> usersRepository)
    : IQueryHandler<GetCartTotalQuery, Result<CartTotalDto>>
{
    public async Task<Result<CartTotalDto>> Handle(GetCartTotalQuery query, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Unauthorized($"User with id: {query.UserId} not found");
        }

        var cart = await cartRepository.GetCartAsync(user.Id, cancellationToken);
        if (cart is null)
        {
            return Result.Error($"Couldn't retrieve user {query.UserId}'s cart");
        }

        var offersByIdsSpec = new OffersByIdsSpec(cart.Items.Select(i => i.OfferId));
        var offers = await offersRepository.ListAsync(offersByIdsSpec, cancellationToken);

        if (offers is null)
        {
            return Result.Error($"Couldn't retrieve offers");
        }

        var itemsWithOffers = cart.Items.Join(offers, i => i.OfferId, o => o.Id, (i, o) => (i, o)).ToList();

        var totalPrice = itemsWithOffers.Sum(t => t.i.Quantity * t.o.Price);

        return Result.Success(new CartTotalDto(totalPrice, "PLN"));
    }
}

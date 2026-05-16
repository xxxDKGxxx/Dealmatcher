namespace Dealmatcher.Backend.UseCases.Features.Cart.Add;

public sealed class AddItemToCartCommandHandler(
  IMapper mapper,
  ICartRepository cartRepository,
  IReadRepository<Offer> offersRepository
) : ICommandHandler<AddItemToCartCommand, Result<CartItemDto>>
{
    public async Task<Result<CartItemDto>> Handle(
      AddItemToCartCommand request,
      CancellationToken cancellationToken
    )
    {
        var spec = new OfferByIdWithDetailsSpec(request.OfferId);
        var addedOffer = await offersRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (addedOffer is null)
        {
            return Result.NotFound("Offer not found");
        }

        if (addedOffer.Seller is null || addedOffer.Seller.Id == request.UserId)
        {
            return Result.Conflict("Cannot add own offer to cart");
        }

        var cart = await cartRepository.GetCartAsync(request.UserId, cancellationToken);

        if (cart.Items.Where(i => i.OfferId == request.OfferId).Any())
        {
            return Result.Conflict("Item already in cart");
        }

        cart.UpdateItemQuantity(request.OfferId, request.Quantity);

        await cartRepository.SaveCartAsync(cart, cancellationToken);

        var addedItem = cart.Items.Single(i => i.OfferId == request.OfferId);
        var addedOfferDto = mapper.Map<OfferDto>(addedOffer);

        return Result.Created(mapper.Map<CartItemDto>((addedItem, addedOfferDto)));
    }
}

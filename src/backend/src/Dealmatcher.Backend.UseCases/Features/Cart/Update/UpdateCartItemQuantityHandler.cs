namespace Dealmatcher.Backend.UseCases.Features.Cart.Update;

public sealed class UpdateCartItemQuantityCommandHandler(
    ICartRepository cartRepository,
    IReadRepository<Offer> offerRepository,
    IMapper mapper) : ICommandHandler<UpdateCartItemQuantityCommand, Result<CartItemDto>>
{
    public async Task<Result<CartItemDto>> Handle(UpdateCartItemQuantityCommand request, CancellationToken ct)
    {
        var cart = await cartRepository.GetCartAsync(request.UserId, ct);

        var existingItem = cart.Items.FirstOrDefault(i => i.OfferId == request.OfferId);
        if (existingItem is null)
        {
            return Result.NotFound("Cart item not found");
        }

        var spec = new OfferByIdWithDetailsSpec(request.OfferId);
        var offer = await offerRepository.FirstOrDefaultAsync(spec, ct);
        if (offer is null)
        {
            return Result.NotFound("Offer not found");
        }

        if (request.Quantity < 1 || request.Quantity > offer.Availability)
        {
            return Result.Invalid(new ValidationError("Invalid quantity"));
        }

        cart.UpdateItemQuantity(request.OfferId, request.Quantity);

        await cartRepository.SaveCartAsync(cart, ct);

        var offerDto = mapper.Map<OfferDto>(offer);

        var updatedItem = cart.Items.First(i => i.OfferId == request.OfferId);

        var responseDto = mapper.Map<CartItemDto>((updatedItem, offerDto));

        return Result.Success(responseDto);
    }
}

namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<(CartItem Item, OfferDto Offer), CartItemDto>()
            .ConstructUsing(src => new CartItemDto(
                src.Item.OfferId,
                src.Offer,
                src.Item.Quantity,
                DateTime.UtcNow
            ));
    }
}

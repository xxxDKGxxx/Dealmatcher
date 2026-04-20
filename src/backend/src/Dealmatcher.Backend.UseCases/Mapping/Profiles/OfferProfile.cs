namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class OfferProfile : Profile
{
    public OfferProfile()
    {
        CreateMap<Offer, OfferDto>()
            .ConstructUsing((src, ctx) => new OfferDto(
                src.Id,
                src.Title,
                src.Description,
                src.Price,
                [.. src.Images],
                ctx.Mapper.Map<SellerDto>(src.Seller),
                ctx.Mapper.Map<CategoryDto>(src.Category),
                [.. src.Tags],
                src.Properties.ToDictionary(p => p.PropertyDefinition.Id.ToString(), p => p.StringValue),
                src.Availability,
                src.Status.Name,
                src.CreatedAt,
                src.UpdatedAt ?? src.CreatedAt));
    }
}

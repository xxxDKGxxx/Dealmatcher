using AutoMapper.Execution;

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
                src.Images.ToList(),
                ctx.Mapper.Map<SellerDto>(src.Seller),
                ctx.Mapper.Map<CategoryDto>(src.Category),
                src.Tags.ToList(),
                src.Properties.Select(p => ctx.Mapper.Map<PropertyDto>(p)).ToList(),
                src.Availability,
                src.Status.Name,
                src.CreatedAt,
                src.UpdatedAt ?? src.CreatedAt));
    }
}

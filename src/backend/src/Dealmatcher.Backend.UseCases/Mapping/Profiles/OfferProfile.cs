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
                [.. src.Images],
                ctx.Mapper.Map<SellerDto>(src.Seller),
                ctx.Mapper.Map<CategoryDto>(src.Category),
                [.. src.Tags],
                src.Properties.ToDictionary(p => p.PropertyDefinition.Id.ToString(), p => p.StringValue),
                src.Availability,
                src.Status.Name,
                src.CreatedAt,
                src.UpdatedAt ?? src.CreatedAt))
            .ForMember(d => d.Properties, o => o.Ignore())
            .ForMember(d => d.Seller, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Tags, o => o.Ignore())
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore());
    }
}

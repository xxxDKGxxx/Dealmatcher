namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class SellerProfile : Profile
{
    public SellerProfile()
    {
        CreateMap<User, SellerDto>()
            .ConstructUsing(src => new SellerDto(src.Id, src.Name));
    }
}

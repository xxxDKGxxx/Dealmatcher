namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public class BanProfile : Profile
{
    public BanProfile()
    {
        CreateMap<Ban, BanDto>()
            .ForCtorParam("UserId", opt => opt.MapFrom(src => src.User.Id))
            .ForCtorParam("IssuedBy", opt => opt.MapFrom(src => src.IssuedBy.Id));
    }
}

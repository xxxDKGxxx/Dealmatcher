namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public class BanProfile : Profile
{
    public BanProfile()
    {
        CreateMap<Ban, BanDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.IssuedBy, opt => opt.MapFrom(src => src.IssuedBy.Id));
    }
}

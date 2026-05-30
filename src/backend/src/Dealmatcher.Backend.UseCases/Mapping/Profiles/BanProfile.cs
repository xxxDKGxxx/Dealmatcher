namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public class BanProfile : Profile
{
    public BanProfile()
    {
        CreateMap<Ban, BanDto>();
    }
}

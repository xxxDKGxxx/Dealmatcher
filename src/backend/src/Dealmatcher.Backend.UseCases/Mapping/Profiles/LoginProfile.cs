namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class LoginProfile : Profile
{
    public LoginProfile()
    {
        CreateMap<(string AccessToken, User User), LoginDto>()
            .ForMember(d => d.AccessToken, o => o.MapFrom(s => s.AccessToken))
            .ForMember(d => d.User, o => o.MapFrom(s => s.User));
    }
}

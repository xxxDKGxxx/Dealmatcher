namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;
public sealed class LoginProfile : Profile
{
    public LoginProfile()
    {
        CreateMap<(string AccessToken, User User), LoginDto>()
            .ConstructUsing((src, ctx) => new LoginDto(
                src.AccessToken,
                ctx.Mapper.Map<UserDto>(src.User)
            ));
    }
}

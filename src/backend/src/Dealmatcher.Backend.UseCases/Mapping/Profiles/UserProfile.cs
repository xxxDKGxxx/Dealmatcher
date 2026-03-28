namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

internal class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>();
    }
}

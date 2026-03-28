namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;
internal class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ConstructUsing(src => new UserDto(
                src.Id,
                src.Email,
                src.Name,
                src.Surname,
                null,
                null,
                null,
                src.Status,
                src.CreatedAt
            ));
    }
}

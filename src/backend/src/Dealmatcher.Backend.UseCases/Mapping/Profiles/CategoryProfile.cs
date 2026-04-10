namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile() 
    {
        CreateMap<Category, CategoryDto>()
            .ConstructUsing(src => new CategoryDto(src.Id, src.Name, src.Description));
    }
}

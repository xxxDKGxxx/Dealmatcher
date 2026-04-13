namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class PropertyProfile : Profile
{
    public PropertyProfile()
    {
        CreateMap<Property, PropertyDto>()
            .ConstructUsing(src => new PropertyDto(src.PropertyDefinition.Name, src.StringValue));
    }
}

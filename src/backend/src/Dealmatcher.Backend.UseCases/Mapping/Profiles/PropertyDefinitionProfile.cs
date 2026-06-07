namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class PropertyDefinitionProfile : Profile
{
    public PropertyDefinitionProfile()
    {
        CreateMap<PropertyDefinition, PropertyDefinitionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString().ToUpper()))
            .ConstructUsing((src, ctx) => new PropertyDefinitionDto(
                src.Id,
                src.Name,
                src.Type.ToString().ToUpper(),
                src is SelectPropertyDefinition spd
                    ? [.. spd.Values]
                    : null
            ));
    }
}

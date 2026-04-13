namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class PropertyDefinitionProfile : Profile
{
    public PropertyDefinitionProfile()
    {
        CreateMap<PropertyDefinition, PropertyDefinitionDto>()
            .ConstructUsing((src, ctx) => new PropertyDefinitionDto(
                src.Id,
                src.Name,
                src.Type.ToString(),
                src is SelectPropertyDefinition spd
                    ? [.. spd.Values]
                    : null
            ));
    }
}

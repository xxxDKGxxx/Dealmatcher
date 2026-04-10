namespace Dealmatcher.Backend.UseCases.Mapping.Profiles;

public sealed class PropertyDefinitionProfile : Profile
{
    public PropertyDefinitionProfile()
    {
        CreateMap<PropertyDefinition, PropertyDefinitionDto>()
            .ConstructUsing((src, ctx) => new PropertyDefinitionDto(
                src.Id,
                src.Name,
                src.Type,
                src is SelectPropertyDefinition spd
                    ? spd.PropertyRelatedEnum.Values.Select(v => v.Value).ToList()
                    : null
            ));
    }
}

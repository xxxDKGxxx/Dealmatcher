using Dealmatcher.Backend.UseCases.Mapping.Profiles;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dealmatcher.Backend.UnitTests.UseCases.Mapping.Profiles;

public class PropertyDefinitionProfileTests
{
    private readonly IMapper _mapper;

    public PropertyDefinitionProfileTests()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<PropertyDefinitionProfile>(),
            NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();
    }

    [Theory]
    [InlineData(PropertyType.Boolean, "BOOLEAN")]
    [InlineData(PropertyType.Numeric, "NUMERIC")]
    [InlineData(PropertyType.Text, "TEXT")]
    public void Map_ProducesUpperCaseType(PropertyType type, string expectedType)
    {
        PropertyDefinition definition = type switch
        {
            PropertyType.Boolean => new BooleanPropertyDefinition("Flag", type),
            PropertyType.Numeric => new NumericPropertyDefinition("Size", type),
            _ => new TextPropertyDefinition("Color", type),
        };

        var dto = _mapper.Map<PropertyDefinitionDto>(definition);

        dto.Type.ShouldBe(expectedType);
    }

    [Fact]
    public void Map_SelectDefinition_ProducesUpperCaseTypeAndOptions()
    {
        var definition = new SelectPropertyDefinition("Brand", PropertyType.Select, ["A", "B"]);

        var dto = _mapper.Map<PropertyDefinitionDto>(definition);

        dto.Type.ShouldBe("SELECT");
        dto.options.ShouldBe(["A", "B"]);
    }
}

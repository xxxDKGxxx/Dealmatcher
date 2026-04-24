namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Categories.Get;

public class GetCategoryPropertiesQueryHandlerTests
{
    private readonly IReadRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;
    private readonly GetCategoryPropertiesQueryHandler _handler;

    public GetCategoryPropertiesQueryHandlerTests()
    {
        _categoryRepository = Substitute.For<IReadRepository<Category>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCategoryPropertiesQueryHandler(_categoryRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ReturnsSuccessWithMappedProperties()
    {
        // Arrange
        var categoryName = "TestCategory";
        var query = new GetCategoryPropertiesQuery(categoryName);
        var category = new Category(categoryName, "Description");

        var propDef1 = new TextPropertyDefinition("Color", PropertyType.Text);
        var propDef2 = new NumericPropertyDefinition("Size", PropertyType.Numeric);

        category.AddPropertyDefinition(propDef1);
        category.AddPropertyDefinition(propDef2);

        var propDto1 = new PropertyDefinitionDto(1, "Color", "Text", null);
        var propDto2 = new PropertyDefinitionDto(2, "Size", "Numeric", null);

        _categoryRepository.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<Category>>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _mapper.Map<PropertyDefinitionDto>(propDef1).Returns(propDto1);
        _mapper.Map<PropertyDefinitionDto>(propDef2).Returns(propDto2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.ShouldContain(propDto1);
        result.Value.ShouldContain(propDto2);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var categoryName = "NonExistentCategory";
        var query = new GetCategoryPropertiesQuery(categoryName);

        _categoryRepository.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<Category>>(), Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}

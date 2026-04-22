namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Categories.Get;

public class GetCategoriesQueryHandlerTests
{
    private readonly IReadRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;
    private readonly GetCategoriesQueryHandler _handler;

    public GetCategoriesQueryHandlerTests()
    {
        _categoryRepository = Substitute.For<IReadRepository<Category>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCategoriesQueryHandler(_categoryRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ReturnsSuccessWithMappedCategories()
    {
        // Arrange
        var category1 = new Category("Category 1", "Description 1");
        var category2 = new Category("Category 2", "Description 2");
        var categories = new List<Category> { category1, category2 };

        var categoryDto1 = new CategoryDto(1, "Category 1", "Description 1");
        var categoryDto2 = new CategoryDto(2, "Category 2", "Description 2");

        _categoryRepository.ListAsync(Arg.Any<CancellationToken>()).Returns(categories);

        _mapper.Map<CategoryDto>(category1).Returns(categoryDto1);
        _mapper.Map<CategoryDto>(category2).Returns(categoryDto2);

        var query = new GetCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.ShouldContain(categoryDto1);
        result.Value.ShouldContain(categoryDto2);
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var categories = new List<Category>();
        _categoryRepository.ListAsync(Arg.Any<CancellationToken>()).Returns(categories);

        var query = new GetCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }
}

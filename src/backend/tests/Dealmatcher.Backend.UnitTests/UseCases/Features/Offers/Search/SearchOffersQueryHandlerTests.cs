// tests/Dealmatcher.Backend.UnitTests/UseCases/Features/Offers/Search/SearchOffersQueryHandlerTests.cs

using Dealmatcher.Backend.Domain.Core.Filtering;
using Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;
using Dealmatcher.Backend.UseCases.Features.Offers.Search;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.Search;

public class SearchOffersQueryHandlerTests
{
    private readonly IReadRepository<Offer> _offerRepository;
    private readonly IReadRepository<Category> _categoryRepository;
    private readonly IOfferSuggestionService _offerSuggestionService;
    private readonly IMapper _mapper;
    private readonly SearchOffersQueryHandler _handler;

    public SearchOffersQueryHandlerTests()
    {
        _offerRepository = Substitute.For<IReadRepository<Offer>>();
        _categoryRepository = Substitute.For<IReadRepository<Category>>();
        _offerSuggestionService = Substitute.For<IOfferSuggestionService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new SearchOffersQueryHandler(_offerRepository, _categoryRepository, _offerSuggestionService, _mapper);
    }

    private static SearchOffersQuery CreateValidQuery(
        int? categoryId = null,
        Dictionary<string, List<string>>? propertyFilters = null)
    {
        return new SearchOffersQuery(
            CategoryId: categoryId,
            MinPrice: 0,
            MaxPrice: 100000,
            Tags: [],
            PropertyFilters: propertyFilters ?? [],
            SearchPhrase: "",
            Limit: 10);
    }

    private static OfferDto CreateDummyOfferDto()
    {
        return new OfferDto(
            1, "Test", "Desc", 100m, [],
            new SellerDto(1, "Test"),
            new CategoryDto(1, "Cars", "Vehicles"),
            [], [],
            1, "ACTIVE", DateTime.UtcNow, DateTime.UtcNow);
    }

    private void SetupOfferRepository(List<Offer> offers)
    {
        _offerRepository.ListAsync(Arg.Any<FilteredOffersSpecification>(), Arg.Any<CancellationToken>())
            .Returns(offers);
    }

    private void SetupSuggestionService(IEnumerable<Offer> output)
    {
        _offerSuggestionService.SuggestOffers(
            Arg.Any<IEnumerable<Offer>>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns(output);
    }

    [Fact]
    public async Task Handle_ValidQueryWithResults_ReturnsSuccess()
    {
        var query = CreateValidQuery();
        var offers = new List<Offer> { CreateDummyOffer(), CreateDummyOffer() };
        var dtos = new List<OfferDto> { CreateDummyOfferDto(), CreateDummyOfferDto() };

        SetupOfferRepository(offers);
        SetupSuggestionService(offers);
        _mapper.Map<List<OfferDto>>(Arg.Any<IEnumerable<Offer>>()).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsNoContent()
    {
        var query = CreateValidQuery();

        SetupOfferRepository([]);
        SetupSuggestionService([]);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NoContent);
    }

    [Fact]
    public async Task Handle_MinPriceGreaterThanMaxPrice_ReturnsInvalid()
    {
        var query = new SearchOffersQuery(
            CategoryId: null,
            MinPrice: 50000,
            MaxPrice: 10000,
            Tags: [],
            PropertyFilters: [],
            SearchPhrase: "",
            Limit: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _offerRepository.DidNotReceive().ListAsync(
            Arg.Any<FilteredOffersSpecification>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCategoryId_CategoryNotFound_ReturnsInvalid()
    {
        var query = CreateValidQuery(categoryId: 999);

        _categoryRepository.SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _offerRepository.DidNotReceive().ListAsync(
            Arg.Any<FilteredOffersSpecification>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCategoryId_ValidCategory_CallsRepository()
    {
        var category = new Category("Cars", "Vehicles");
        var query = CreateValidQuery(categoryId: 1);

        _categoryRepository.SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>())
            .Returns(category);
        SetupOfferRepository([]);
        SetupSuggestionService([]);

        await _handler.Handle(query, CancellationToken.None);

        await _offerRepository.Received(1).ListAsync(
            Arg.Any<FilteredOffersSpecification>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutCategoryId_SkipsCategoryValidation()
    {
        var query = CreateValidQuery(categoryId: null);

        SetupOfferRepository([]);
        SetupSuggestionService([]);

        await _handler.Handle(query, CancellationToken.None);

        await _categoryRepository.DidNotReceive().SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidPropertyId_ReturnsInvalid()
    {
        var category = new Category("Cars", "Vehicles");
        var query = CreateValidQuery(
            categoryId: 1,
            propertyFilters: new Dictionary<string, List<string>>
            {
                ["not-a-number"] = ["value"]
            });

        _categoryRepository.SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_PropertyIdNotInCategory_ReturnsInvalid()
    {
        var category = new Category("Cars", "Vehicles");
        var query = CreateValidQuery(
            categoryId: 1,
            propertyFilters: new Dictionary<string, List<string>>
            {
                ["999"] = ["value"]
            });

        _categoryRepository.SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task Handle_PassesLimitToSuggestionService()
    {
        var query = new SearchOffersQuery(
            CategoryId: null,
            MinPrice: 0,
            MaxPrice: 100000,
            Tags: [],
            PropertyFilters: [],
            SearchPhrase: "",
            Limit: 25);

        var offers = new List<Offer> { CreateDummyOffer() };
        SetupOfferRepository(offers);
        SetupSuggestionService(offers);
        _mapper.Map<List<OfferDto>>(Arg.Any<IEnumerable<Offer>>())
            .Returns([CreateDummyOfferDto()]);

        await _handler.Handle(query, CancellationToken.None);

        await _offerSuggestionService.Received(1).SuggestOffers(
            Arg.Any<IEnumerable<Offer>>(),
            25,
            Arg.Any<CancellationToken>());
    }

    private static Offer CreateDummyOffer()
    {
        var category = new Category("Cars", "Vehicles");
        var user = new User("test@test.com", "hash", "Test", "User");
        return new Offer("Test", "Desc", 100m, [], user, [], 1, category, []);
    }
}

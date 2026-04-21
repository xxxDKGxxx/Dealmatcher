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

    private static SearchOffersQuery CreateValidQuery(int? categoryId = null)
    {
        return new SearchOffersQuery(
            CategoryId: categoryId,
            MinPrice: 0,
            MaxPrice: 100000,
            Tags: [],
            SearchPhrase: "",
            limit: 10);
    }

    [Fact]
    public async Task Handle_ValidQueryWithResults_ReturnsSuccess()
    {
        var query = CreateValidQuery();
        var offers = new List<Offer> { CreateDummyOffer(), CreateDummyOffer() };
        var dtos = new List<OfferDto> { CreateDummyOfferDto(), CreateDummyOfferDto() };

        _offerSuggestionService.SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>())
            .Returns(offers);
        _mapper.Map<List<OfferDto>>(offers).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsNoContent()
    {
        var query = CreateValidQuery();

        _offerSuggestionService.SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>())
            .Returns([]);

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
            SearchPhrase: "",
            limit: 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _offerSuggestionService.DidNotReceive().SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCategoryId_ValidCategory_ReturnsSuccess()
    {
        var category = new Category("Cars", "Vehicles");
        var query = CreateValidQuery(categoryId: 1);
        var offers = new List<Offer> { CreateDummyOffer() };
        var dtos = new List<OfferDto> { CreateDummyOfferDto() };

        _categoryRepository.SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>())
            .Returns(category);
        _offerSuggestionService.SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>())
            .Returns(offers);
        _mapper.Map<List<OfferDto>>(offers).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
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
        await _offerSuggestionService.DidNotReceive().SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutCategoryId_SkipsCategoryValidation()
    {
        var query = CreateValidQuery(categoryId: null);
        var offers = new List<Offer> { CreateDummyOffer() };
        var dtos = new List<OfferDto> { CreateDummyOfferDto() };

        _offerSuggestionService.SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>())
            .Returns(offers);
        _mapper.Map<List<OfferDto>>(offers).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _categoryRepository.DidNotReceive().SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesCorrectParametersToSuggestionService()
    {
        var query = new SearchOffersQuery(
            CategoryId: 5,
            MinPrice: 100,
            MaxPrice: 5000,
            Tags: ["used", "cheap"],
            SearchPhrase: "bmw",
            limit: 20);

        var category = new Category("Cars", "Vehicles");
        _categoryRepository.SingleOrDefaultAsync(
            Arg.Any<CategoryWithDefinitionsByIdSpec>(),
            Arg.Any<CancellationToken>())
            .Returns(category);
        _offerSuggestionService.SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Any<OfferSearchParameters>(),
            Arg.Any<CancellationToken>())
            .Returns([]);

        await _handler.Handle(query, CancellationToken.None);

        await _offerSuggestionService.Received(1).SuggestOffers(
            Arg.Any<IReadRepository<Offer>>(),
            Arg.Is<OfferSearchParameters>(p =>
                p.CategoryId == 5 &&
                p.MinPrice == 100 &&
                p.MaxPrice == 5000 &&
                p.Tags.Count == 2 &&
                p.SearchPhrase == "bmw" &&
                p.Limit == 20),
            Arg.Any<CancellationToken>());
    }

    private static Offer CreateDummyOffer()
    {
        var category = new Category("Cars", "Vehicles");
        var user = new User("test@test.com", "hash", "Test", "User");
        return new Offer("Test", "Desc", 100m, [], user, [], 1, category, []);
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
}

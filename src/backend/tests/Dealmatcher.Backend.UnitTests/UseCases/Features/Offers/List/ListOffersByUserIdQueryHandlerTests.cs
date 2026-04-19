namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.List;

public class ListOffersByUserIdQueryHandlerTests
{
    private readonly IReadRepository<Offer> _offersRepository;
    private readonly IReadRepository<User> _usersRepository;
    private readonly IMapper _mapper;
    private readonly ListOffersByUserIdQueryHandler _handler;

    public ListOffersByUserIdQueryHandlerTests()
    {
        _offersRepository = Substitute.For<IReadRepository<Offer>>();
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListOffersByUserIdQueryHandler(_offersRepository, _usersRepository, _mapper);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var query = new ListOffersByUserIdQuery(userId);
        _usersRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Errors.ShouldContain($"User with id {userId} not found");
    }

    [Fact]
    public async Task Handle_UserExistsAndHasOffers_ReturnsSuccessWithOffers()
    {
        // Arrange
        var userId = 1;
        var query = new ListOffersByUserIdQuery(userId);
        var user = new User("test@example.com", "hash", "Jan", "Kowalski");

        var category = new Category("Test Category", "Desc");
        var offer1 = new Offer("Title 1", "Desc 1", 100m, [], user, [], 1, category, []);
        var offer2 = new Offer("Title 2", "Desc 2", 200m, [], user, [], 1, category, []);
        var offers = new List<Offer> { offer1, offer2 };

        _usersRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _offersRepository.ListAsync(Arg.Any<ISpecification<Offer>>(), Arg.Any<CancellationToken>())
            .Returns(offers);

        _mapper.Map<List<OfferDto>>(Arg.Any<List<Offer>>())
            .Returns(x => [.. ((List<Offer>)x[0]).Select(CreateOfferDto)]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Title.ShouldBe("Title 1");
        result.Value[1].Title.ShouldBe("Title 2");
    }

    [Fact]
    public async Task Handle_UserExistsButNoOffers_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var userId = 1;
        var query = new ListOffersByUserIdQuery(userId);
        var user = new User("test@example.com", "hash", "Jan", "Kowalski");

        _usersRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _offersRepository.ListAsync(Arg.Any<ISpecification<Offer>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _mapper.Map<List<OfferDto>>(Arg.Any<List<Offer>>())
            .Returns([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }

    private static OfferDto CreateOfferDto(Offer offer)
    {
        return new OfferDto(
            offer.Id,
            offer.Title,
            offer.Description,
            offer.Price,
            [.. offer.Images],
            new SellerDto(offer.Seller.Id, offer.Seller.Name),
            new CategoryDto(offer.Category.Id, offer.Category.Name, offer.Category.Description),
            [.. offer.Tags],
            offer.Properties.ToDictionary(p => p.PropertyDefinition.Id.ToString(), p => p.StringValue),
            offer.Availability,
            "ACTIVE",
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }
}

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.Update;

public class UpdateOfferCommandHandlerTests
{
    private readonly IRepository<Offer> _offerRepository;
    private readonly IReadRepository<Category> _categoryRepository;
    private readonly IReadRepository<Purchase> _purchaseRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;
    private readonly UpdateOfferCommandHandler _handler;

    private readonly User _seller;
    private readonly Offer _offer;

    private static readonly int _przebiegId = 1;
    private static readonly int _uszkodzonyId = 2;
    private static readonly int _markaId = 3;

    public UpdateOfferCommandHandlerTests()
    {
        _offerRepository = Substitute.For<IRepository<Offer>>();
        _categoryRepository = Substitute.For<IReadRepository<Category>>();
        _purchaseRepository = Substitute.For<IReadRepository<Purchase>>();
        _imageStorageService = Substitute.For<IImageStorageService>();
        _mapper = Substitute.For<IMapper>();

        _handler = new UpdateOfferCommandHandler(
            _offerRepository,
            _categoryRepository,
            _purchaseRepository,
            _imageStorageService,
            _mapper);

        _purchaseRepository.ListAsync(Arg.Any<PendingPurchasesByOfferIdSpec>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _seller = new User("seller@example.com", "hash", "Jan", "Kowalski");

        var category = new Category("Elektronika", "Kategoria elektroniki");
        typeof(Category).GetProperty("Id")?.SetValue(category, 1);

        var przebieg = new NumericPropertyDefinition("Przebieg", PropertyType.Numeric) { Id = _przebiegId };
        var uszkodzony = new BooleanPropertyDefinition("Uszkodzony", PropertyType.Boolean) { Id = _uszkodzonyId };
        var marka = new SelectPropertyDefinition("Marka", PropertyType.Select, ["Apple", "Samsung", "Sony"]) { Id = _markaId };

        category.AddPropertyDefinition(przebieg);
        category.AddPropertyDefinition(uszkodzony);
        category.AddPropertyDefinition(marka);

        List<string> initialImages = ["https://blob.com/img1.jpg", "https://blob.com/img2.jpg"];

        _offer = new Offer("Stary Tytuł", "Stary Opis", 100m, initialImages, _seller, ["stary-tag"], 1, category, []);

        typeof(Offer).GetProperty("Id")?.SetValue(_offer, 1);
        typeof(User).GetProperty("Id")?.SetValue(_seller, 1);

        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);

        _categoryRepository.FirstOrDefaultAsync(Arg.Any<CategoryWithDefinitionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _mapper.Map<OfferDto>(Arg.Any<Offer>()).Returns(new OfferDto(1, "Nowy Tytuł", "Opis", 150m, [], null!, null!, [], null!, 5, "DRAFT", DateTime.UtcNow, DateTime.UtcNow));
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesBasicFieldsAndSaves()
    {
        // Arrange
        var command = new UpdateOfferCommand(
            OfferId: 1,
            UserId: 1,
            Title: "Nowy Tytuł",
            Description: "Nowy Opis",
            Price: 150m,
            Images: null,
            Tags: ["nowy-tag"],
            Properties: null,
            Availability: 5);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _offer.Title.ShouldBe("Nowy Tytuł");
        _offer.Description.ShouldBe("Nowy Opis");
        _offer.Price.ShouldBe(150m);
        _offer.Availability.ShouldBe(5);
        _offer.Tags.ShouldContain("nowy-tag");
        _offer.Status.ShouldBe(OfferStatus.Draft);

        await _offerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RemovedImages_CallsDeleteImageAsyncOnStorageService()
    {
        List<string> newImages = ["https://blob.com/img2.jpg"];
        var command = new UpdateOfferCommand(1, 1, null, null, null, newImages, null, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _imageStorageService.Received(1).DeleteImageAsync("https://blob.com/img1.jpg", Arg.Any<CancellationToken>());
        await _imageStorageService.DidNotReceive().DeleteImageAsync("https://blob.com/img2.jpg", Arg.Any<CancellationToken>());

        _offer.Images.Count.ShouldBe(1);
        _offer.Images.ShouldContain("https://blob.com/img2.jpg");
    }

    [Fact]
    public async Task Handle_UserIsNotSeller_ReturnsForbidden()
    {
        // Arrange
        var command = new UpdateOfferCommand(OfferId: 1, UserId: 999, "Title", null, null, null, null, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _offerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        // Arrange
        _offerRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Offer?)null);
        var command = new UpdateOfferCommand(OfferId: 99, UserId: 1, "Title", null, null, null, null, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_ValidProperties_UpdatesProperties()
    {
        // Arrange
        var command = new UpdateOfferCommand(1, 1, null, null, null, null, null, new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "24",
            [_uszkodzonyId.ToString()] = "false",
            [_markaId.ToString()] = "Samsung"
        }, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _offer.Properties.Count.ShouldBe(3);
        await _offerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingProperties_UpdatesExistingInstances()
    {
        // Arrange
        var category = _offer.Category;
        var przebiegDef = category.PropertyDefinitions.First(pd => pd.Id == _przebiegId);
        var initialProperty = przebiegDef.CreatePropertyFromString("10");

        _offer.SetProperties([initialProperty]);

        var command = new UpdateOfferCommand(1, 1, null, null, null, null, null, new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "20",
            [_uszkodzonyId.ToString()] = "true",
            [_markaId.ToString()] = "Apple"
        }, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _offer.Properties.Count.ShouldBe(3);
        var updatedProperty = _offer.Properties.First(p => p.PropertyDefinition.Id == _przebiegId);

        updatedProperty.ShouldBeSameAs(initialProperty);
        updatedProperty.StringValue.ShouldBe("20");

        await _offerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MissingProperties_ReturnsInvalid()
    {
        // Arrange
        var command = new UpdateOfferCommand(1, 1, null, null, null, null, null, new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "24"
        }, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Missing required properties"));
        await _offerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidPropertyValue_ReturnsInvalid()
    {
        // Arrange
        var command = new UpdateOfferCommand(1, 1, null, null, null, null, null, new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "nie-liczba",
            [_uszkodzonyId.ToString()] = "false",
            [_markaId.ToString()] = "Samsung"
        }, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid property value"));
        await _offerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPropertyId_ReturnsInvalid()
    {
        // Arrange
        var command = new UpdateOfferCommand(1, 1, null, null, null, null, null, new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "24",
            [_uszkodzonyId.ToString()] = "false",
            [_markaId.ToString()] = "Samsung",
            ["NieistniejącaProperty"] = "123"
        }, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid property Id"));
        await _offerRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

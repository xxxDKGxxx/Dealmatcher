using Dealmatcher.Backend.UseCases.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.Create;

public class CreateOfferCommandHandlerTests
{
    private readonly IReadRepository<User> _userRepository;
    private readonly IReadRepository<Category> _categoryRepository;
    private readonly IRepository<Offer> _offerRepository;
    private readonly IImageStorageService _imageStorageService;
    private readonly IMapper _mapper;
    private readonly CreateOfferCommandHandler _handler;

    private static readonly User _validUser = new("seller@example.com", "hash", "Jan", "Kowalski");
    private static readonly Category _validCategory = new("Samochody", "Kategoria samochodów");
    private static readonly int _przebiegId = 1;
    private static readonly int _uszkodzonyId = 2;
    private static readonly int _markaId = 3;

    public CreateOfferCommandHandlerTests()
    {
        _userRepository = Substitute.For<IReadRepository<User>>();
        _categoryRepository = Substitute.For<IReadRepository<Category>>();
        _offerRepository = Substitute.For<IRepository<Offer>>();
        _imageStorageService = Substitute.For<IImageStorageService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateOfferCommandHandler(
            _userRepository,
            _categoryRepository,
            _offerRepository,
            _imageStorageService,
            _mapper);
    }

    private static Category CreateCategoryWithDefinitions()
    {
        var category = new Category("Samochody", "Kategoria samochodów");
        var przebieg = new NumericPropertyDefinition("Przebieg", PropertyType.Number)
        {
            Id = _przebiegId
        };
        var uszkodzony = new BooleanPropertyDefinition("Uszkodzony", PropertyType.Boolean)
        {
            Id = _uszkodzonyId
        };
        var marka = new SelectPropertyDefinition("Marka", PropertyType.Select, ["BMW", "Audi", "Mercedes"])
        {
            Id = _markaId
        };

        category.AddPropertyDefinition(przebieg);
        category.AddPropertyDefinition(uszkodzony);
        category.AddPropertyDefinition(marka);
        return category;
    }

    private static CreateOfferCommand CreateValidCommand(Dictionary<string, string>? properties = null, List<FileDto>? images = null)
    {
        return new CreateOfferCommand(
            Title: "BMW E46",
            Description: "Dobry stan",
            Price: 25000m,
            Images: images ?? [],
            SellerId: 1,
            CategoryId: 1,
            Tags: ["samochód", "bmw"],
            Properties: properties ?? new Dictionary<string, string>
            {
                [_przebiegId.ToString()] = "180000",
                [_uszkodzonyId.ToString()] = "false",
                [_markaId.ToString()] = "BMW"
            },
            Availability: 1);
    }

    private void SetupValidUser()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(_validUser);
    }

    private Category SetupValidCategory()
    {
        var category = CreateCategoryWithDefinitions();
        _categoryRepository.FirstOrDefaultAsync(Arg.Any<CategoryWithDefinitionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(category);
        return category;
    }

    private void SetupMapper()
    {
        _mapper.Map<OfferDto>(Arg.Any<Offer>())
            .Returns(callInfo =>
            {
                var offer = callInfo.Arg<Offer>();
                return new OfferDto(
                    offer.Id, offer.Title, offer.Description, offer.Price,
                    [.. offer.Images],
                    new SellerDto(0, "Jan"),
                    new CategoryDto(0, "Samochody", "Kategoria samochodów"),
                    [.. offer.Tags],
                    offer.Properties.ToDictionary(p => p.PropertyDefinition.Name, p => p.StringValue),
                    offer.Availability,
                    "ACTIVE",
                    DateTime.UtcNow,
                    DateTime.UtcNow);
            });
    }

    [Fact]
    public async Task Handle_ValidData_ReturnsSuccess()
    {
        SetupValidUser();
        SetupValidCategory();
        SetupMapper();
        var command = CreateValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe("BMW E46");
        result.Value.Properties.Count.ShouldBe(3);
        await _offerRepository.Received(1).AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
        await _offerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidDataWithImages_CallsImageStorageServiceAndReturnsSuccess()
    {
        SetupValidUser();
        SetupValidCategory();
        SetupMapper();

        using var dummyStream = new MemoryStream();
        List<FileDto> images = [new(dummyStream, "auto.jpg", "image/jpeg")];

        var command = CreateValidCommand(images: images);

        _imageStorageService
            .UploadImageAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("https://azure-blob.com/auto.jpg");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        await _imageStorageService.Received(1).UploadImageAsync(
            dummyStream,
            "auto.jpg",
            "image/jpeg",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsInvalid()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var command = CreateValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsInvalid()
    {
        SetupValidUser();
        _categoryRepository.FirstOrDefaultAsync(Arg.Any<CategoryWithDefinitionsByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Category?)null);
        var command = CreateValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPropertyName_ReturnsInvalid()
    {
        SetupValidUser();
        SetupValidCategory();

        var command = CreateValidCommand(new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "180000",
            [_uszkodzonyId.ToString()] = "false",
            [_markaId.ToString()] = "BMW",
            ["NieistniejącaProperty"] = "123"
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid property Id"));
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidNumericValue_ReturnsInvalid()
    {
        SetupValidUser();
        SetupValidCategory();
        var command = CreateValidCommand(new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "nie-liczba",
            [_uszkodzonyId.ToString()] = "false",
            [_markaId.ToString()] = "BMW"
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid property value"));
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidBooleanValue_ReturnsInvalid()
    {
        SetupValidUser();
        SetupValidCategory();
        var command = CreateValidCommand(new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "180000",
            [_uszkodzonyId.ToString()] = "nie-bool",
            [_markaId.ToString()] = "BMW"
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid property value"));
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidSelectValue_ReturnsInvalid()
    {
        SetupValidUser();
        SetupValidCategory();
        var command = CreateValidCommand(new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "180000",
            [_uszkodzonyId.ToString()] = "false",
            [_markaId.ToString()] = "Toyota"
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Invalid property value"));
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyProperties_ReturnsInvalid()
    {
        SetupValidUser();
        SetupValidCategory();
        var command = CreateValidCommand([]);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Missing required properties"));
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MissingSomeProperties_ReturnsInvalid()
    {
        SetupValidUser();
        SetupValidCategory();
        var command = CreateValidCommand(new Dictionary<string, string>
        {
            [_przebiegId.ToString()] = "180000"
        });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage.Contains("Missing required properties"));
        await _offerRepository.DidNotReceive().AddAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidData_DoesNotCallCategoryRepoBeforeUserValidation()
    {
        _userRepository.FirstOrDefaultAsync(Arg.Any<ActiveUserByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        var command = CreateValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _categoryRepository.DidNotReceive()
            .FirstOrDefaultAsync(Arg.Any<CategoryWithDefinitionsByIdSpec>(), Arg.Any<CancellationToken>());
    }
}

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.Get;

public class GetOfferQueryHandlerTests
{
    private readonly IReadRepository<Offer> _offerRepository;
    private readonly IMapper _mapper;
    private readonly GetOfferQueryHandler _handler;

    private const int ValidOfferId = 1;

    public GetOfferQueryHandlerTests()
    {
        _offerRepository = Substitute.For<IReadRepository<Offer>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetOfferQueryHandler(_offerRepository, _mapper);
    }

    private static Offer CreateValidOffer(bool isDeleted = false)
    {
        var user = new User("test@example.com", "hash", "Jan", "Kowalski");
        var category = new Category("Test Category", "Desc");
        var offer = new Offer("Test Offer", "Test Description", 1500m, [], user, [], 1, category, []);

        if (isDeleted)
        {
            offer.Delete();
        }

        return offer;
    }

    private static OfferDto CreateOfferDto()
    {
        return new OfferDto(
            ValidOfferId,
            "Test Offer",
            "Test Description",
            1500m,
            [],
            new SellerDto(1, "Jan"),
            new CategoryDto(1, "Test Category", "Desc"),
            [],
            [],
            1,
            "ACTIVE",
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    [Fact]
    public async Task Handle_OfferExists_ReturnsSuccess()
    {
        var query = new GetOfferQuery(ValidOfferId);
        var offer = CreateValidOffer();
        var expectedDto = CreateOfferDto();

        _offerRepository.FirstOrDefaultAsync(Arg.Is<ISpecification<Offer>>(s => s is OfferByIdWithDetailsSpec), Arg.Any<CancellationToken>())
            .Returns(offer);

        _mapper.Map<OfferDto>(offer)
            .Returns(expectedDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);
        result.Value.Id.ShouldBe(ValidOfferId);
        result.Value.Title.ShouldBe("Test Offer");
    }

    [Fact]
    public async Task Handle_OfferDoesNotExist_ReturnsNotFound()
    {
        var query = new GetOfferQuery(ValidOfferId);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Offer>>(), Arg.Any<CancellationToken>())
            .Returns((Offer?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _mapper.DidNotReceive().Map<OfferDto>(Arg.Any<Offer>());
    }

    [Fact]
    public async Task Handle_OfferIsDeleted_ReturnsNotFound()
    {
        var query = new GetOfferQuery(ValidOfferId);
        var deletedOffer = CreateValidOffer(isDeleted: true);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Offer>>(), Arg.Any<CancellationToken>())
            .Returns(deletedOffer);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        _mapper.DidNotReceive().Map<OfferDto>(Arg.Any<Offer>());
    }

    [Fact]
    public async Task Handle_ValidQuery_CallsRepositoryWithCorrectSpecification()
    {
        var query = new GetOfferQuery(ValidOfferId);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<ISpecification<Offer>>(), Arg.Any<CancellationToken>())
            .Returns((Offer?)null);

        await _handler.Handle(query, CancellationToken.None);

        await _offerRepository.Received(1).FirstOrDefaultAsync(
            Arg.Is<ISpecification<Offer>>(s => s is OfferByIdWithDetailsSpec),
            Arg.Any<CancellationToken>());
    }
}

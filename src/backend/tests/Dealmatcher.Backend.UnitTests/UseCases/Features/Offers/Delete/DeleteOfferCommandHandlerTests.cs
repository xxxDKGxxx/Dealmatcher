namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.Delete;

public class DeleteOfferCommandHandlerTests
{
    private readonly IRepository<Offer> _offerRepository;
    private readonly DeleteOfferCommandHandler _handler;

    private readonly User _seller;
    private readonly Offer _offer;

    public DeleteOfferCommandHandlerTests()
    {
        _offerRepository = Substitute.For<IRepository<Offer>>();
        _handler = new DeleteOfferCommandHandler(_offerRepository);

        _seller = new User("seller@example.com", "hash", "Jan", "Kowalski");
        var category = new Category("Samochody", "Opis");

        _offer = new Offer(
            "Testowa oferta",
            "Opis",
            100m,
            [],
            _seller,
            [],
            1,
            category,
            []);

        typeof(Offer).GetProperty("Id")?.SetValue(_offer, 1);
        typeof(User).GetProperty("Id")?.SetValue(_seller, 1);
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesOfferAndReturnsSuccess()
    {
        // Arrange
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);
        var command = new DeleteOfferCommand(OfferId: 1, UserId: 1, IsAdmin: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _offerRepository.Received(1).DeleteAsync(_offer, Arg.Any<CancellationToken>());
        await _offerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AdminDeletesOtherUserOffer_ReturnsSuccess()
    {
        // Arrange
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);

        var command = new DeleteOfferCommand(OfferId: 1, UserId: 999, IsAdmin: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _offerRepository.Received(1).DeleteAsync(_offer, Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        // Arrange
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Offer?)null);
        var command = new DeleteOfferCommand(OfferId: 1, UserId: 1, IsAdmin: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
        await _offerRepository.DidNotReceive().DeleteAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserIsNotSellerAndNotAdmin_ReturnsForbidden()
    {
        // Arrange
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);

        var command = new DeleteOfferCommand(OfferId: 1, UserId: 999, IsAdmin: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _offerRepository.DidNotReceive().DeleteAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }
}

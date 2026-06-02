namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Offers.Delete;

public class DeleteOfferCommandHandlerTests
{
    private readonly IRepository<Offer> _offerRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IReadRepository<Purchase> _purchaseRepository;
    private readonly DeleteOfferCommandHandler _handler;
    private readonly User _seller;
    private readonly Offer _offer;

    public DeleteOfferCommandHandlerTests()
    {
        _offerRepository = Substitute.For<IRepository<Offer>>();
        _userRepository = Substitute.For<IRepository<User>>();
        _purchaseRepository = Substitute.For<IReadRepository<Purchase>>();
        _purchaseRepository.ListAsync(Arg.Any<PendingPurchasesByOfferIdSpec>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _handler = new DeleteOfferCommandHandler(_offerRepository, _userRepository, _purchaseRepository);

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
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);
        var command = new DeleteOfferCommand(OfferId: 1, UserId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _offerRepository.Received(1).DeleteAsync(_offer, Arg.Any<CancellationToken>());
        await _offerRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AdminDeletesOtherUserOffer_ReturnsSuccess()
    {
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);

        var adminUser = new User("admin@example.com", "hash", "Jan", "Kowalski");
        typeof(User).GetProperty("Id")?.SetValue(adminUser, 999);
        adminUser.GrantAdminPrivileges();

        _userRepository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns(adminUser);
        var command = new DeleteOfferCommand(OfferId: 1, UserId: 999);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _offerRepository.Received(1).DeleteAsync(_offer, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OfferNotFound_ReturnsNotFound()
    {
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((Offer?)null);
        var command = new DeleteOfferCommand(OfferId: 1, UserId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
        await _offerRepository.DidNotReceive().DeleteAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserIsNotSellerAndNotAdmin_ReturnsForbidden()
    {
        _offerRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(_offer);
        var command = new DeleteOfferCommand(OfferId: 1, UserId: 999);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Forbidden);
        await _offerRepository.DidNotReceive().DeleteAsync(Arg.Any<Offer>(), Arg.Any<CancellationToken>());
    }
}

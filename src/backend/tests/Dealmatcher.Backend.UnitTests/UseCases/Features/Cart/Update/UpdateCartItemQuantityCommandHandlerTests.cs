using CartEntity = Dealmatcher.Backend.Domain.Core.Cart.Cart;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Cart.Update;

public class UpdateCartItemQuantityCommandHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IReadRepository<Offer> _offerRepository;
    private readonly IMapper _mapper;
    private readonly UpdateCartItemQuantityCommandHandler _handler;

    public UpdateCartItemQuantityCommandHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _offerRepository = Substitute.For<IReadRepository<Offer>>();
        _mapper = Substitute.For<IMapper>();

        _handler = new UpdateCartItemQuantityCommandHandler(
            _cartRepository,
            _offerRepository,
            _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessAndUpdatedItem_WhenAllConditionsAreMet()
    {
        // Arrange
        var userId = 1;
        var offerId = 100;
        var initialQuantity = 1;
        var newQuantity = 5;

        var command = new UpdateCartItemQuantityCommand(userId, offerId, newQuantity);

        var cart = new CartEntity(userId);
        cart.UpdateItemQuantity(offerId, initialQuantity);

        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>()).Returns(cart);

        var offer = new Offer("Test", "Desc", 100m, [], null!, [], 5, null!, []);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .Returns(offer);

        var offerDto = new OfferDto(offerId, "Test", "Desc", 100m, [], null!, null!, [], [], 1, "Active", DateTime.UtcNow, DateTime.UtcNow);
        _mapper.Map<OfferDto>(offer).Returns(offerDto);

        var expectedCartItemDto = new CartItemDto(offerId, offerDto, newQuantity, DateTime.UtcNow);
        _mapper.Map<CartItemDto>(Arg.Any<(CartItem, OfferDto)>()).Returns(expectedCartItemDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedCartItemDto);
        result.Value.Quantity.ShouldBe(newQuantity);

        await _cartRepository.Received(1).SaveCartAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemDoesNotExistInCart()
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(UserId: 1, OfferId: 999, Quantity: 2);

        var cart = new CartEntity(command.UserId);
        _cartRepository.GetCartAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Errors.ShouldContain("Cart item not found");

        await _cartRepository.DidNotReceive().SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOfferDoesNotExistInDatabase()
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(UserId: 1, OfferId: 100, Quantity: 2);

        var cart = new CartEntity(command.UserId);
        cart.UpdateItemQuantity(command.OfferId, 1);

        _cartRepository.GetCartAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(cart);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Errors.ShouldContain("Offer not found");

        await _cartRepository.DidNotReceive().SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task Handle_ShouldReturnInvalid_WhenQuantityExceedsAvailability()
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(UserId: 1, OfferId: 100, Quantity: 5);

        var cart = new CartEntity(command.UserId);
        cart.UpdateItemQuantity(command.OfferId, 1);
        _cartRepository.GetCartAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(cart);

        var offer = new Offer("Test", "Desc", 100m, [], null!, [], 2, null!, []);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .Returns(offer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Invalid);

        await _cartRepository.DidNotReceive().SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalid_WhenQuantityIsLessThanOne()
    {
        // Arrange
        var command = new UpdateCartItemQuantityCommand(UserId: 1, OfferId: 100, Quantity: 0);

        var cart = new CartEntity(command.UserId);
        cart.UpdateItemQuantity(command.OfferId, 1);
        _cartRepository.GetCartAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(cart);

        var offer = new Offer("Test", "Desc", 100m, [], null!, [], 10, null!, []);

        _offerRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>())
            .Returns(offer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Invalid);

        await _cartRepository.DidNotReceive().SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }
}

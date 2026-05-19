using Dealmatcher.Backend.UseCases.Features.Cart.Add;

using CartEntity = Dealmatcher.Backend.Domain.Core.Cart.Cart;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Cart.Add;

public class AddItemToCartCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly ICartRepository _cartRepository;
    private readonly IReadRepository<Offer> _offersRepository;
    private readonly AddItemToCartCommandHandler _handler;

    public AddItemToCartCommandHandlerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _cartRepository = Substitute.For<ICartRepository>();
        _offersRepository = Substitute.For<IReadRepository<Offer>>();

        _handler = new AddItemToCartCommandHandler(_mapper, _cartRepository, _offersRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessAndAddedItem_WhenOfferExistsAndItemIsNotInCart()
    {
        var userId = 1;
        var offerId = 100;
        var quantity = 1;
        var command = new AddItemToCartCommand(userId, offerId, quantity);

        var cart = new CartEntity(userId);
        var seller = new User("seller@test.com", "hash", "Name", "Surname") { Id = 2 };
        var category = new Category("Test", "Desc");
        var offer = new Offer("Test", "Desc", 100m, [], seller, [], 5, category, []);
        var offerDto = new OfferDto(offerId, "Test", "Desc", 100m, [], null!, null!, [], [], 1, "Active", DateTime.UtcNow, DateTime.UtcNow);
        var expectedCartItemDto = new CartItemDto(offerId, offerDto, quantity, DateTime.UtcNow);

        _offersRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>()).Returns(offer);
        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<OfferDto>(offer).Returns(offerDto);
        _mapper.Map<CartItemDto>(Arg.Any<(CartItem, OfferDto)>()).Returns(expectedCartItemDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedCartItemDto);
        result.Value.Quantity.ShouldBe(quantity);
        await _cartRepository.Received(1).SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOfferDoesNotExist()
    {
        // Arrange
        var userId = 1;
        var offerId = 100;
        var quantity = 1;
        var command = new AddItemToCartCommand(userId, offerId, quantity);

        _offersRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Errors.ShouldContain("Offer not found");
        await _cartRepository
          .DidNotReceive()
          .SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenItemAlreadyInCart()
    {
        // Arrange
        var userId = 1;
        var offerId = 100;
        var quantity = 1;
        var command = new AddItemToCartCommand(userId, offerId, quantity);

        var existingCart = new CartEntity(userId);
        existingCart.UpdateItemQuantity(offerId, 1); // Add item to cart to simulate conflict

        var seller = new User("seller@test.com", "hash", "Name", "Surname") { Id = 2 };
        var category = new Category("Test", "Desc");
        var offer = new Offer("Test", "Desc", 100m, [], seller, [], 5, category, []);

        _offersRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>()).Returns(offer);
        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>()).Returns(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Conflict);
        result.Errors.ShouldContain("Item already in cart");
        await _cartRepository
          .DidNotReceive()
          .SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenAddingOwnOffer()
    {
        // Arrange
        var userId = 1;
        var offerId = 100;
        var quantity = 1;
        var command = new AddItemToCartCommand(userId, offerId, quantity);

        var seller = new User("email@test.com", "hash", "Name", "Surname") { Id = userId };
        var category = new Category("Test", "Desc");
        var offer = new Offer("Test", "Desc", 100m, [], seller, [], 5, category, []);

        _offersRepository.FirstOrDefaultAsync(Arg.Any<OfferByIdWithDetailsSpec>(), Arg.Any<CancellationToken>()).Returns(offer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Conflict);
        result.Errors.ShouldContain("Cannot add own offer to cart");
        await _cartRepository
          .DidNotReceive()
          .SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }
}

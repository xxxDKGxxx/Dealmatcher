using Ardalis.Result;
using Dealmatcher.Backend.Domain.Core.Cart;
using Dealmatcher.Backend.Domain.Core.Cart.Dto;
using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;
using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Dto;
using Dealmatcher.Backend.Domain.Interfaces;
using Dealmatcher.Backend.UseCases.Features.Cart.Add;
using Dealmatcher.Backend.UseCases.Mapping;
using NSubstitute;
using Shouldly;
using Xunit;
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

        _handler = new AddItemToCartCommandHandler(
            _mapper,
            _cartRepository,
            _offersRepository
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessAndAddedItem_WhenOfferExistsAndItemIsNotInCart()
    {
        // Arrange
        var userId = 1;
        var offerId = 100;
        var quantity = 1;
        var command = new AddItemToCartCommand(userId, offerId, quantity);

        var existingCart = new CartEntity(userId);
        var offer = new Offer("Test", "Desc", 100m, [], null!, [], 5, null!, []);
        var offerDto = new OfferDto(offerId, "Test", "Desc", 100m, [], null!, null!, [], [], 1, "Active", DateTime.UtcNow, DateTime.UtcNow);

        // This updatedCart is what the handler will return after saving the cart
        var updatedCart = new CartEntity(userId);
        updatedCart.UpdateItemQuantity(offerId, quantity);
        var itemInUpdatedCart = updatedCart.Items.Single(i => i.OfferId == offerId);

        var expectedCartItemDto = new CartItemDto(offerId, offerDto, quantity, DateTime.UtcNow);

        _offersRepository.GetByIdAsync(offerId, Arg.Any<CancellationToken>()).Returns(offer);

        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>())
            .Returns(existingCart, updatedCart);

        _mapper.Map<CartItemDto>((itemInUpdatedCart, offer)).Returns(expectedCartItemDto);
        _mapper.Map<OfferDto>(offer).Returns(offerDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
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

        _offersRepository.GetByIdAsync(offerId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Errors.ShouldContain("Offer not found");
        await _cartRepository.DidNotReceive().SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
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

        var offer = new Offer("Test", "Desc", 100m, [], null!, [], 5, null!, []);

        _offersRepository.GetByIdAsync(offerId, Arg.Any<CancellationToken>()).Returns(offer);
        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>()).Returns(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Conflict);
        result.Errors.ShouldContain("Item already in cart");
        await _cartRepository.DidNotReceive().SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }
}

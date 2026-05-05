using Ardalis.Result;
using Dealmatcher.Backend.Domain.Interfaces;
using Dealmatcher.Backend.UseCases.Features.Cart.Delete;
using NSubstitute;
using Shouldly;
using Xunit;
using CartEntity = Dealmatcher.Backend.Domain.Core.Cart.Cart;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Cart.Delete;

public class DeleteItemFromCartCommandHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly DeleteItemFromCartCommandHandler _handler;

    public DeleteItemFromCartCommandHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new DeleteItemFromCartCommandHandler(_cartRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoContent_WhenItemExistsInCart()
    {
        // Arrange
        var userId = 1;
        var cartItemId = 100;
        var command = new DeleteItemFromCartCommand(cartItemId, userId);

        var cart = new CartEntity(userId);
        cart.UpdateItemQuantity(cartItemId, 1);

        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Status.ShouldBe(ResultStatus.NoContent);

        cart.Items.ShouldBeEmpty();

        await _cartRepository
          .Received(1)
          .SaveCartAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenItemNotInCart()
    {
        // Arrange
        var userId = 1;
        var cartItemId = 100;
        var command = new DeleteItemFromCartCommand(cartItemId, userId);

        var cart = new CartEntity(userId);

        _cartRepository.GetCartAsync(userId, Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.NotFound);
        result.Errors.ShouldContain("Cart item not found");

        await _cartRepository
          .DidNotReceive()
          .SaveCartAsync(Arg.Any<CartEntity>(), Arg.Any<CancellationToken>());
    }
}

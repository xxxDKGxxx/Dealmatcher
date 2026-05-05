using CartEntity = Dealmatcher.Backend.Domain.Core.Cart.Cart;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Cart.GetTotal;
public class GetCartTotalQueryHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IReadRepository<Offer> _offersRepository;
    private readonly IReadRepository<User> _usersRepository;
    private readonly GetCartTotalQueryHandler _handler;

    public GetCartTotalQueryHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _offersRepository = Substitute.For<IReadRepository<Offer>>();
        _usersRepository = Substitute.For<IReadRepository<User>>();
        _handler = new GetCartTotalQueryHandler(_cartRepository, _offersRepository, _usersRepository);
    }

    private static User CreateUser(int id = 1)
    {
        var user = new User("test@example.com", "hash", "Test", "User")
        {
            Id = id
        };
        return user;
    }

    private static Offer CreateOffer(int id, decimal price)
    {
        var category = new Category("Cars", "Vehicles");
        var seller = new User("seller@example.com", "hash", "Seller", "User");
        var offer = new Offer("Test", "Desc", price, [], seller, [], 1, category, [])
        {
            Id = id
        };
        return offer;
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUnauthorized()
    {
        _usersRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(new GetCartTotalQuery(99), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        await _cartRepository.DidNotReceive().GetCartAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CartIsNull_ReturnsError()
    {
        var user = CreateUser();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns((CartEntity)null!);

        var result = await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_EmptyCart_ReturnsZeroTotal()
    {
        var user = CreateUser();
        var cart = new CartEntity(1);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var result = await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.TotalPrice.ShouldBe(0m);
        result.Value.Currency.ShouldBe("PLN");
    }

    [Fact]
    public async Task Handle_SingleItem_ReturnsCorrectTotal()
    {
        var user = CreateUser();
        var cart = new CartEntity(1);
        cart.UpdateItemQuantity(10, 2);
        var offer = CreateOffer(10, 150m);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([offer]);

        var result = await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.TotalPrice.ShouldBe(300m);
    }

    [Fact]
    public async Task Handle_MultipleItems_ReturnsSumTotal()
    {
        var user = CreateUser();
        var cart = new CartEntity(1);
        cart.UpdateItemQuantity(10, 2);
        cart.UpdateItemQuantity(20, 3);
        var offer1 = CreateOffer(10, 100m);
        var offer2 = CreateOffer(20, 50m);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([offer1, offer2]);

        var result = await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.TotalPrice.ShouldBe(350m);
    }

    [Fact]
    public async Task Handle_OffersIsNull_ReturnsError()
    {
        var user = CreateUser();
        var cart = new CartEntity(1);
        cart.UpdateItemQuantity(10, 1);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        await _offersRepository.DidNotReceive().ListAsync(Arg.Any<ISpecification<Offer>>(), Arg.Any<CancellationToken>());

        var result = await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Error);
    }

    [Fact]
    public async Task Handle_CurrencyIsPLN()
    {
        var user = CreateUser();
        var cart = new CartEntity(1);
        cart.UpdateItemQuantity(10, 1);
        var offer = CreateOffer(10, 99m);

        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([offer]);

        var result = await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Currency.ShouldBe("PLN");
    }

    [Fact]
    public async Task Handle_UserNotFound_DoesNotCallCart()
    {
        _usersRepository.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        await _handler.Handle(new GetCartTotalQuery(99), CancellationToken.None);

        await _cartRepository.DidNotReceive().GetCartAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _offersRepository.DidNotReceive().ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CartNull_DoesNotCallOffers()
    {
        var user = CreateUser();
        _usersRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns((CartEntity)null!);

        await _handler.Handle(new GetCartTotalQuery(1), CancellationToken.None);

        await _offersRepository.DidNotReceive().ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>());
    }
}

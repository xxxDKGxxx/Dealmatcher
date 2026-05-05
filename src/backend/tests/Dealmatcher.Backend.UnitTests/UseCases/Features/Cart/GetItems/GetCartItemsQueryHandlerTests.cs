using CartEntity = Dealmatcher.Backend.Domain.Core.Cart.Cart;

namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Cart.GetItems;

public class GetCartItemsQueryHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IReadRepository<Offer> _offersRepository;
    private readonly IMapper _mapper;
    private readonly GetCartItemsQueryHandler _handler;

    public GetCartItemsQueryHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _offersRepository = Substitute.For<IReadRepository<Offer>>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCartItemsQueryHandler(_cartRepository, _offersRepository, _mapper);
    }

    private static Offer CreateOffer(int id, string title = "Test", decimal price = 100m)
    {
        var category = new Category("Cars", "Vehicles");
        var seller = new User("seller@example.com", "hash", "Seller", "User");
        var offer = new Offer(title, "Desc", price, [], seller, [], 1, category, [])
        {
            Id = id
        };
        return offer;
    }

    private static OfferDto CreateOfferDto(int id, string title = "Test", decimal price = 100m)
    {
        return new OfferDto(id, title, "Desc", price, [], null!, null!, [], [], 1, "ACTIVE", DateTime.UtcNow, DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_EmptyCart_ReturnsSuccessWithEmptyList()
    {
        var cart = new CartEntity(1);
        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>()).Returns([]);

        var result = await _handler.Handle(new GetCartItemsQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_CartWithItems_ReturnsSuccessWithMappedItems()
    {
        var cart = new CartEntity(1);
        cart.UpdateItemQuantity(10, 2);
        cart.UpdateItemQuantity(20, 1);

        var offer1 = CreateOffer(10, "BMW E46", 15000m);
        var offer2 = CreateOffer(20, "Audi A4", 20000m);
        var offerDto1 = CreateOfferDto(10, "BMW E46", 15000m);
        var offerDto2 = CreateOfferDto(20, "Audi A4", 20000m);
        var cartItemDto1 = new CartItemDto(10, offerDto1, 2, DateTime.UtcNow);
        var cartItemDto2 = new CartItemDto(20, offerDto2, 1, DateTime.UtcNow);

        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([offer1, offer2]);
        _mapper.Map<OfferDto>(offer1).Returns(offerDto1);
        _mapper.Map<OfferDto>(offer2).Returns(offerDto2);
        _mapper.Map<CartItemDto>(Arg.Any<(CartItem, OfferDto)>())
            .Returns(callInfo =>
            {
                var tuple = callInfo.Arg<(CartItem, OfferDto)>();
                return tuple.Item2.Id == 10 ? cartItemDto1 : cartItemDto2;
            });

        var result = await _handler.Handle(new GetCartItemsQuery(1), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_CartWithItems_CallsRepositoryWithCorrectIds()
    {
        var cart = new CartEntity(1);
        cart.UpdateItemQuantity(10, 1);
        cart.UpdateItemQuantity(20, 2);

        var offer1 = CreateOffer(10);
        var offer2 = CreateOffer(20);

        _cartRepository.GetCartAsync(1, Arg.Any<CancellationToken>()).Returns(cart);
        _offersRepository.ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>())
            .Returns([offer1, offer2]);
        _mapper.Map<OfferDto>(Arg.Any<Offer>()).Returns(CreateOfferDto(0));
        _mapper.Map<CartItemDto>(Arg.Any<(CartItem, OfferDto)>()).Returns(new CartItemDto(0, null!, 0, DateTime.UtcNow));

        await _handler.Handle(new GetCartItemsQuery(1), CancellationToken.None);

        await _offersRepository.Received(1).ListAsync(Arg.Any<OffersByIdsSpec>(), Arg.Any<CancellationToken>());
    }
}

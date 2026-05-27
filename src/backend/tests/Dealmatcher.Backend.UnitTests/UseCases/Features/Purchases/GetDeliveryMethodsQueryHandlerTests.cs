namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Purchases;

public class GetDeliveryMethodsQueryHandlerTests
{
    private readonly IDeliveryProviderService _deliveryProviderService;
    private readonly IMapper _mapper;
    private readonly GetDeliveryMethodsQueryHandler _handler;

    public GetDeliveryMethodsQueryHandlerTests()
    {
        _deliveryProviderService = Substitute.For<IDeliveryProviderService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetDeliveryMethodsQueryHandler(_deliveryProviderService, _mapper);
    }

    [Fact]
    public async Task Handle_ProvidersExist_ReturnsSuccessWithMappedDtosAndEstimatedDays()
    {
        // Arrange
        var provider1 = Substitute.For<IDeliveryProvider>();
        provider1.Id.Returns("inpost");
        provider1.GetEstimatedDaysAsync(Arg.Any<DeliveryContext>()).Returns(Task.FromResult(2));

        var provider2 = Substitute.For<IDeliveryProvider>();
        provider2.Id.Returns("dpd");
        provider2.GetEstimatedDaysAsync(Arg.Any<DeliveryContext>()).Returns(Task.FromResult(1));

        var providers = new List<IDeliveryProvider> { provider1, provider2 }.AsReadOnly();
        _deliveryProviderService.GetAllDeliveryProviders().Returns(providers);

        var dto1 = new DeliveryMethodDto("inpost", "InPost", "Paczkomat 24/7", 14.99m, 0);
        var dto2 = new DeliveryMethodDto("dpd", "Kurier DPD", "Dostawa do drzwi", 19.99m, 0);

        _mapper.Map<DeliveryMethodDto>(provider1).Returns(dto1);
        _mapper.Map<DeliveryMethodDto>(provider2).Returns(dto2);

        var query = new GetDeliveryMethodsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);

        var inpostResult = result.Value.First(x => x.Id == "inpost");
        inpostResult.EstimatedDays.ShouldBe(2);
        inpostResult.Price.ShouldBe(14.99m);

        var dpdResult = result.Value.First(x => x.Id == "dpd");
        dpdResult.EstimatedDays.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_NoProviders_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var providers = new List<IDeliveryProvider>().AsReadOnly();
        _deliveryProviderService.GetAllDeliveryProviders().Returns(providers);

        // Act
        var result = await _handler.Handle(new GetDeliveryMethodsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }
}

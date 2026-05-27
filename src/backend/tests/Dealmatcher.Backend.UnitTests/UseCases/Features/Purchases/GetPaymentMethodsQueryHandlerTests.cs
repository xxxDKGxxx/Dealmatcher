namespace Dealmatcher.Backend.UnitTests.UseCases.Features.Purchases;

public class GetPaymentMethodsQueryHandlerTests
{
    private readonly IPaymentProviderService _paymentProviderService;
    private readonly IMapper _mapper;
    private readonly GetPaymentMethodsQueryHandler _handler;

    public GetPaymentMethodsQueryHandlerTests()
    {
        _paymentProviderService = Substitute.For<IPaymentProviderService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetPaymentMethodsQueryHandler(_paymentProviderService, _mapper);
    }

    [Fact]
    public async Task Handle_ProvidersExist_ReturnsSuccessWithMappedDtos()
    {
        var provider1 = Substitute.For<IPaymentProvider>();
        provider1.Id.Returns("stripe");
        provider1.Name.Returns("Stripe");

        var provider2 = Substitute.For<IPaymentProvider>();
        provider2.Id.Returns("payu");
        provider2.Name.Returns("PayU");

        var providers = new List<IPaymentProvider> { provider1, provider2 }.AsReadOnly();
        _paymentProviderService.GetAllPaymentProviders().Returns(providers);

        var dto1 = new PaymentMethodDto("stripe", "Stripe", "stripe", "stripe-icon");
        var dto2 = new PaymentMethodDto("payu", "PayU", "payu", "payu-icon");
        _mapper.Map<PaymentMethodDto>(provider1).Returns(dto1);
        _mapper.Map<PaymentMethodDto>(provider2).Returns(dto2);

        var result = await _handler.Handle(new GetPaymentMethodsQuery(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Id.ShouldBe("stripe");
        result.Value[1].Id.ShouldBe("payu");
    }

    [Fact]
    public async Task Handle_NoProviders_ReturnsSuccessWithEmptyList()
    {
        var providers = new List<IPaymentProvider>().AsReadOnly();
        _paymentProviderService.GetAllPaymentProviders().Returns(providers);

        var result = await _handler.Handle(new GetPaymentMethodsQuery(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_CallsServiceAndMapper()
    {
        var provider = Substitute.For<IPaymentProvider>();
        provider.Id.Returns("example");
        var providers = new List<IPaymentProvider> { provider }.AsReadOnly();
        _paymentProviderService.GetAllPaymentProviders().Returns(providers);
        _mapper.Map<PaymentMethodDto>(provider).Returns(new PaymentMethodDto("example", "Example", "example", "icon"));

        await _handler.Handle(new GetPaymentMethodsQuery(), CancellationToken.None);

        _paymentProviderService.Received(1).GetAllPaymentProviders();
        _mapper.Received(1).Map<PaymentMethodDto>(provider);
    }
}

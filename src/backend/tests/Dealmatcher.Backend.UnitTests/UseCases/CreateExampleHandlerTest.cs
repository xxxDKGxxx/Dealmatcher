//namespace Dealmatcher.Backend.UnitTests.UseCases;
//public class CreateExampleHandlerTest
//{
//    private readonly IRepository<ExampleEntity> _exampleRepository;
//    private readonly IMapper _mapper;
//    private readonly CreateExampleCommandHandler _handler;

//    public CreateExampleHandlerTest()
//    {
//        _exampleRepository = Substitute.For<IRepository<ExampleEntity>>();
//        _mapper = Substitute.For<IMapper>();
//        _handler = new CreateExampleCommandHandler(_exampleRepository, _mapper);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnSuccess_Example()
//    {
//        var command = new CreateExampleCommand(1);
//        var example = new ExampleEntity(1);
//        var dto = new ExampleDto(1, 1);

//        _exampleRepository.AddAsync(Arg.Any<ExampleEntity>()).Returns(example);
//        _mapper.Map<ExampleDto>(example).Returns(dto);

//        var result = await _handler.Handle(command, CancellationToken.None);

//        result.IsSuccess.ShouldBeTrue();
//        result.Value.ShouldBe(dto);
//        await _exampleRepository.Received(1).AddAsync(Arg.Is<ExampleEntity>(e => e != null), Arg.Any<CancellationToken>());
//        _mapper.Received(1).Map<ExampleDto>(example);
//    }
//}

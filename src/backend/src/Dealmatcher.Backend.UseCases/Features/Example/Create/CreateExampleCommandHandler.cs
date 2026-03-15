namespace Dealmatcher.Backend.UseCases.Features.Example.Create;

public sealed class CreateExampleCommandHandler(
    IRepository<ExampleEntity> exampleRepository,
    IMapper mapper) : ICommandHandler<CreateExampleCommand, Result<ExampleDto>>
{
    public async Task<Result<ExampleDto>> Handle(CreateExampleCommand request, CancellationToken cancellationToken)
    {
        ExampleEntity example = new ExampleEntity(request.E);
        await exampleRepository.AddAsync(example, cancellationToken);
        return mapper.Map<ExampleDto>(example);
    }
}

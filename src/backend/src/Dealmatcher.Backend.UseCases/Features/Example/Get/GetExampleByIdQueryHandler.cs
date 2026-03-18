//namespace Dealmatcher.Backend.UseCases.Features.Example.Get;

//public sealed class GetExampleByIdQueryHandler(
//    IReadRepository<ExampleEntity> exampleRepository,
//    IMapper mapper) :
//    IQueryHandler<GetExampleByIdQuery, Result<ExampleDto>>
//{
//    public async Task<Result<ExampleDto>> Handle(GetExampleByIdQuery request, CancellationToken cancellationToken)
//    {
//        var example = await exampleRepository.GetByIdAsync(request.ExampleId, cancellationToken);

//        if (example is null)
//        {
//            return Result.NotFound();
//        }

//        return Result.Success(mapper.Map<ExampleDto>(example));
//    }
//}

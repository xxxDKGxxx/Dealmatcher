using Ardalis.Result;
using Dealmatcher.Backend.Domain.EntityAggregates.ExampleAggregate.Dto;

namespace Dealmatcher.Backend.UseCases.Features.Example.Create;

public sealed record CreateExampleCommand(
    int E
) : ICommand<Result<ExampleDto>>;

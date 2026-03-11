namespace Dealmatcher.Backend.Domain.EntityAggregates.ExampleAggregate;

public sealed class Example(int e) : DealmatcherEntityBase, IAggregateRoot
{
    public int E { get; private set; } = e;
}

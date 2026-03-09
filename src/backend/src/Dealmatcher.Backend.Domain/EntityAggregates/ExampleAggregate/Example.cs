namespace Dealmatcher.Backend.Domain.EntityAggregates.ExampleAggregate;
public sealed class Example : DealmatcherEntityBase, IAggregateRoot
{
    public int E { get; private set; }
    public Example(int e)
    {
        E = e;
    }
}

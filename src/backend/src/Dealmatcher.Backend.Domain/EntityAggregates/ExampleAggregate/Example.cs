namespace Dealmatcher.Backend.Domain.EntityAggregates.ExampleAggregate;

public sealed class Example(int e) : DealmatcherEntityBase, IAggregateRoot
{
    public int E { get; private set; } = e;

    public override void Delete()
    {
        throw new NotImplementedException();
    }
}

namespace Dealmatcher.Backend.Domain.EntityAggregates.PropertyAggregate;

internal class SelectProperty : Property
{
    public int Value { get; private set; }

    public SelectProperty(int propertyDefinitionId, int offerId, int value) : base(propertyDefinitionId, offerId)
    {
        Value = value;
    }
    private SelectProperty() { }

    public void SetValue(int value)
    {
        Value = value;
    }
    public override void Delete()
    {
    }
}

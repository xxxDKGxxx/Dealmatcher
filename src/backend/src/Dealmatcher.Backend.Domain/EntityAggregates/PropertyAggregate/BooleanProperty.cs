namespace Dealmatcher.Backend.Domain.EntityAggregates.PropertyAggregate;

internal class BooleanProperty : Property
{
    public bool Value { get; private set; }

    public BooleanProperty(int propertyDefinitionId, int offerId, bool value) : base(propertyDefinitionId, offerId)
    {
        Value = value;
    }
    private BooleanProperty() { }

    public void SetValue(bool value)
    {
        Value = value;
    }
    public override void Delete()
    {
    }
}

namespace Dealmatcher.Backend.Domain.EntityAggregates.PropertyAggregate;

public sealed class NumericProperty : Property
{
    public double Value { get; private set; }

    public NumericProperty(int propertyDefinitionId, int offerId, double value) : base(propertyDefinitionId, offerId)
    {
        Value = value;
    }
    private NumericProperty() { }
    
    public void SetValue(double value)
    {
        Value = value;
    }
    public override void Delete()
    {
    }
}

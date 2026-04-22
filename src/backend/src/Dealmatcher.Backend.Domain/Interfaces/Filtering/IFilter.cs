namespace Dealmatcher.Backend.Domain.Interfaces.Filtering;

public interface IFilter
{
    void ApplyFilter(ISpecificationBuilder<Offer> query);
}

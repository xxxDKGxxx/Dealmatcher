using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;

namespace Dealmatcher.Backend.FunctionalTests;

public class TestModelCustomizer(ModelCustomizerDependencies dependencies)
    : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        // 1. Wywołujemy bazowe budowanie (to odpali ApplyConfigurationsFromAssembly z Twojego kodu produkcji)
        base.Customize(modelBuilder, context);

        // 2. Twardo nadpisujemy QueryFilter tylko dla testów!
        var activeStatus = UserStatus.Active;

        modelBuilder
            .Entity<Offer>() // <-- Zmień na nazwę swojej encji
            .HasQueryFilter(o => o.Seller.Status == activeStatus);
    }
}

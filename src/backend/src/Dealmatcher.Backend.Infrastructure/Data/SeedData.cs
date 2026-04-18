namespace Dealmatcher.Backend.Infrastructure.Data;

public static class SeedData
{
    private static void AddCategories(AppDbContext dbContext)
    {
        if (!dbContext.Set<Category>().Any())
        {
            var samochody = new Category("Samochody", "Pojazdy osobowe");
            samochody.AddPropertyDefinition(new SelectPropertyDefinition("Marka", PropertyType.Select, ["BMW", "Audi", "Mercedes", "Toyota", "Volkswagen"]));
            samochody.AddPropertyDefinition(new NumericPropertyDefinition("Przebieg", PropertyType.Numeric));
            samochody.AddPropertyDefinition(new NumericPropertyDefinition("Rok produkcji", PropertyType.Numeric));
            samochody.AddPropertyDefinition(new BooleanPropertyDefinition("Uszkodzony", PropertyType.Boolean));

            var telefony = new Category("Telefony", "Smartfony i akcesoria");
            telefony.AddPropertyDefinition(new SelectPropertyDefinition("Marka", PropertyType.Select, ["Apple", "Samsung", "Xiaomi", "Huawei", "Google"]));
            telefony.AddPropertyDefinition(new NumericPropertyDefinition("Pamięć GB", PropertyType.Numeric));
            telefony.AddPropertyDefinition(new BooleanPropertyDefinition("Gwarancja", PropertyType.Boolean));

            var ubrania = new Category("Ubrania", "Odzież");
            ubrania.AddPropertyDefinition(new SelectPropertyDefinition("Typ", PropertyType.Select, ["Bluzka", "Spodnie", "Skarpetki"]));
            ubrania.AddPropertyDefinition(new SelectPropertyDefinition("Rozmiar", PropertyType.Select, ["S", "M", "L"]));
            ubrania.AddPropertyDefinition(new TextPropertyDefinition("Marka", PropertyType.Text));

            dbContext.Set<Category>().AddRange(samochody, telefony, ubrania);
        }
    }
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        AddCategories(dbContext);

        await dbContext.SaveChangesAsync();
    }

    public static async Task InitializeTestAsync(AppDbContext dbContext)
    {
        await InitializeAsync(dbContext);
    }
}

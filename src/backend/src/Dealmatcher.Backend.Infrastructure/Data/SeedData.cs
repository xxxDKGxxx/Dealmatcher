namespace Dealmatcher.Backend.Infrastructure.Data;

public static class SeedData
{
    private static void AddCategories(AppDbContext dbContext)
    {
        if (!dbContext.Set<Category>().Any())
        {
            var cars = new Category("Cars", "Personal and work vehicles");
            cars.AddPropertyDefinition(new SelectPropertyDefinition("Brand", PropertyType.Select, ["BMW", "Audi", "Mercedes", "Toyota", "Volkswagen"]));
            cars.AddPropertyDefinition(new NumericPropertyDefinition("Mileage", PropertyType.Numeric));
            cars.AddPropertyDefinition(new NumericPropertyDefinition("Year of production", PropertyType.Numeric));
            cars.AddPropertyDefinition(new BooleanPropertyDefinition("Damaged", PropertyType.Boolean));

            var phones = new Category("Phones", "Smartphones and accessories");
            phones.AddPropertyDefinition(new SelectPropertyDefinition("Brand", PropertyType.Select, ["Apple", "Samsung", "Xiaomi", "Huawei", "Google"]));
            phones.AddPropertyDefinition(new NumericPropertyDefinition("Storage GB", PropertyType.Numeric));
            phones.AddPropertyDefinition(new BooleanPropertyDefinition("Warranty", PropertyType.Boolean));

            var clothing = new Category("Clothing", "Apparel");
            clothing.AddPropertyDefinition(new SelectPropertyDefinition("Type", PropertyType.Select, ["Shirt", "Pants", "Socks"]));
            clothing.AddPropertyDefinition(new SelectPropertyDefinition("Size", PropertyType.Select, ["S", "M", "L"]));
            clothing.AddPropertyDefinition(new TextPropertyDefinition("Brand", PropertyType.Text));

            dbContext.Set<Category>().AddRange(cars, phones, clothing);
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

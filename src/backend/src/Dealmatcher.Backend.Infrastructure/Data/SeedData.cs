namespace Dealmatcher.Backend.Infrastructure.Data;

public static class SeedData
{
    private static void AddCategories(AppDbContext dbContext)
    {
        if (!dbContext.Set<Category>().Any())
        {
            var cars = new Category("Cars", "Personal and work vehicles");
            cars.AddPropertyDefinition(new SelectPropertyDefinition("Brand", PropertyType.Select, ["BMW", "Audi", "Mercedes", "Toyota", "Volkswagen"]));
            cars.AddPropertyDefinition(new NumericPropertyDefinition("Mileage", PropertyType.Number));
            cars.AddPropertyDefinition(new NumericPropertyDefinition("Year of production", PropertyType.Number));
            cars.AddPropertyDefinition(new BooleanPropertyDefinition("Damaged", PropertyType.Boolean));

            var phones = new Category("Phones", "Smartphones and accessories");
            phones.AddPropertyDefinition(new SelectPropertyDefinition("Brand", PropertyType.Select, ["Apple", "Samsung", "Xiaomi", "Huawei", "Google"]));
            phones.AddPropertyDefinition(new NumericPropertyDefinition("Storage GB", PropertyType.Number));
            phones.AddPropertyDefinition(new BooleanPropertyDefinition("Warranty", PropertyType.Boolean));

            var clothing = new Category("Clothing", "Apparel");
            clothing.AddPropertyDefinition(new SelectPropertyDefinition("Type", PropertyType.Select, ["Shirt", "Pants", "Socks"]));
            clothing.AddPropertyDefinition(new SelectPropertyDefinition("Size", PropertyType.Select, ["S", "M", "L"]));
            clothing.AddPropertyDefinition(new TextPropertyDefinition("Brand", PropertyType.Text));

            dbContext.Set<Category>().AddRange(cars, phones, clothing);
        }
    }

    private static void AddUsers(AppDbContext dbContext)
    {
        if (!dbContext.Set<User>().Any())
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");
            var users = new List<User>
            {
                new("john.doe@example.com", passwordHash, "John", "Doe"),
                new("jane.smith@example.com", passwordHash, "Jane", "Smith"),
                new("admin@dealmatcher.com", passwordHash, "Admin", "User")
            };
            users[2].GrantAdminPrivileges();

            dbContext.Set<User>().AddRange(users);
        }
    }

    private static async Task AddOffers(AppDbContext dbContext, IImageStorageService? imageStorageService)
    {
        if (dbContext.Set<Offer>().Any()) return;

        var users = dbContext.Set<User>().ToList();
        var categories = dbContext.Set<Category>()
            .Include(c => c.PropertyDefinitions)
            .ToList();

        if (users.Count == 0 || categories.Count == 0) return;

        var faker = new Faker();
        using var client = new HttpClient();
        var offers = new List<Offer>();

        foreach (var category in categories)
        {
            var keywords = category.Name.ToLower();
            for (int i = 0; i < 5; i++)
            {
                var seller = faker.PickRandom(users);

                string title;
                string description;
                decimal price;
                var properties = new List<Property>();

                if (category.Name == "Cars")
                {
                    var brandDef = (SelectPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Brand");
                    var brand = faker.PickRandom(brandDef.Values.ToList());
                    var model = faker.Vehicle.Model();
                    var year = faker.Random.Int(2010, 2024);

                    title = $"{brand} {model} ({year})";
                    description = $"Well-maintained {brand} {model} from {year}. {faker.Lorem.Paragraph()}";
                    price = faker.Random.Decimal(5000, 80000);

                    properties.Add(new SelectProperty(brandDef, brand));
                    properties.Add(new NumericProperty((NumericPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Mileage"), faker.Random.Double(1000, 300000)));
                    properties.Add(new NumericProperty((NumericPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Year of production"), year));
                    properties.Add(new BooleanProperty((BooleanPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Damaged"), faker.Random.Bool(0.1f)));
                }
                else if (category.Name == "Phones")
                {
                    var brandDef = (SelectPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Brand");
                    var brand = faker.PickRandom(brandDef.Values.ToList());
                    var model = brand switch
                    {
                        "Apple" => $"iPhone {faker.Random.Int(12, 15)} Pro",
                        "Samsung" => $"Galaxy S{faker.Random.Int(21, 24)} Ultra",
                        "Google" => $"Pixel {faker.Random.Int(6, 8)} Pro",
                        _ => faker.Commerce.ProductName()
                    };

                    title = $"{brand} {model}";
                    description = $"Amazing {brand} {model} in great condition. {faker.Lorem.Sentence()}";
                    price = faker.Random.Decimal(300, 1500);

                    properties.Add(new SelectProperty(brandDef, brand));
                    properties.Add(new NumericProperty((NumericPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Storage GB"), faker.PickRandom(new double[] { 64, 128, 256, 512 })));
                    properties.Add(new BooleanProperty((BooleanPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Warranty"), faker.Random.Bool(0.7f)));
                }
                else if (category.Name == "Clothing")
                {
                    var typeDef = (SelectPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Type");
                    var sizeDef = (SelectPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Size");
                    var type = faker.PickRandom(typeDef.Values.ToList());
                    var size = faker.PickRandom(sizeDef.Values.ToList());
                    var brandName = faker.Company.CompanyName();

                    title = $"{brandName} {type} - Size {size}";
                    description = $"Stylish and comfortable {type.ToLower()} by {brandName}. Material: Cotton. {faker.Lorem.Sentence()}";
                    price = faker.Random.Decimal(20, 200);

                    properties.Add(new SelectProperty(typeDef, type));
                    properties.Add(new SelectProperty(sizeDef, size));
                    properties.Add(new TextProperty((TextPropertyDefinition)category.PropertyDefinitions.First(d => d.Name == "Brand"), brandName));
                }
                else
                {
                    title = faker.Commerce.ProductName();
                    description = faker.Commerce.ProductDescription();
                    price = decimal.Parse(faker.Commerce.Price(10, 5000));
                    properties.AddRange(category.PropertyDefinitions.Select(def => GenerateProperty(def, faker)));
                }

                var images = new List<string>();
                var imageCount = faker.Random.Int(1, 5);

                string imageSearchKeywords = category.Name switch
                {
                    "Cars" => title.Split(' ')[0] + ",car,street",
                    "Phones" => "smartphone,tech,desk",
                    "Clothing" => "fashion," + title.Split(' ')[0],
                    _ => keywords
                };

                for (int j = 0; j < imageCount; j++)
                {
                    if (imageStorageService != null)
                    {
                        try
                        {
                            var imageUrl = $"https://loremflickr.com/800/600/{imageSearchKeywords}?lock={faker.Random.Int(1, 10000)}";
                            var imageBytes = await client.GetByteArrayAsync(imageUrl);
                            using var ms = new MemoryStream(imageBytes);
                            var uploadedUrl = await imageStorageService.UploadImageAsync(ms, $"{keywords}-{faker.Random.Guid()}.jpg", "image/jpeg");
                            images.Add(uploadedUrl);
                        }
                        catch
                        {
                            images.Add(faker.Image.LoremFlickrUrl(keywords: imageSearchKeywords));
                        }
                    }
                    else
                    {
                        images.Add(faker.Image.LoremFlickrUrl(keywords: imageSearchKeywords));
                    }
                }

                var offer = new Offer(
                    title: title,
                    description: description,
                    price: price,
                    images: images,
                    seller: seller,
                    tags: [.. faker.Commerce.Categories(3)],
                    availability: faker.Random.Int(1, 50),
                    category: category,
                    properties: properties
                );
                offers.Add(offer);
            }
        }

        dbContext.Set<Offer>().AddRange(offers);
    }

    private static Property GenerateProperty(PropertyDefinition def, Faker faker)
    {
        return def switch
        {
            SelectPropertyDefinition sDef => new SelectProperty(sDef, faker.PickRandom(sDef.Values.ToList())),
            NumericPropertyDefinition nDef => new NumericProperty(nDef, faker.Random.Double(0, 1000)),
            BooleanPropertyDefinition bDef => new BooleanProperty(bDef, faker.Random.Bool()),
            TextPropertyDefinition tDef => new TextProperty(tDef, faker.Lorem.Word()),
            _ => throw new NotSupportedException($"Property definition type {def.GetType().Name} is not supported")
        };
    }

    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        AddCategories(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public static async Task InitializeTestAsync(AppDbContext dbContext, IImageStorageService? imageStorageService = null)
    {
        await InitializeAsync(dbContext);

        AddUsers(dbContext);
        await dbContext.SaveChangesAsync();

        await AddOffers(dbContext, imageStorageService);
        await dbContext.SaveChangesAsync();
    }
}

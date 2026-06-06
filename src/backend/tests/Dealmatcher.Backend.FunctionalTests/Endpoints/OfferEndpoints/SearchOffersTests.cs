using Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate.Properties;

namespace Dealmatcher.Backend.FunctionalTests.Endpoints.OfferEndpoints;

public class SearchOffersTests(CustomWebApplicationFactory factory) : EndpointTestBase(factory)
{
    private async Task CreateOfferInDb(string sellerEmail, string title, decimal price, Category category, Dictionary<string, string> propertyValues, List<string>? tags = null)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var seller = await db.Set<User>().FirstAsync(u => u.Email == sellerEmail);
        var trackedCategory = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync(c => c.Id == category.Id);

        List<Property> properties = [];
        foreach (var (propId, value) in propertyValues)
        {
            var definition = trackedCategory.PropertyDefinitions.First(pd => pd.Id == int.Parse(propId));
            properties.Add(definition.CreatePropertyFromString(value));
        }

        var offer = new Offer(title, "Test description", price, [], seller, tags ?? [], 1, trackedCategory, properties);
        db.Set<Offer>().Add(offer);
        await db.SaveChangesAsync();
    }

    private async Task<(Category category, int mileageId, int damagedId, int brandId)> GetCarsCategory()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var category = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync(c => c.Name == "Cars");
        var mileageId = category.PropertyDefinitions.First(pd => pd.Name == "Mileage").Id;
        var damagedId = category.PropertyDefinitions.First(pd => pd.Name == "Damaged").Id;
        var brandId = category.PropertyDefinitions.First(pd => pd.Name == "Brand").Id;
        return (category, mileageId, damagedId, brandId);
    }

    private async Task<HttpResponseMessage> SearchOffers(
        int? categoryId = null,
        decimal minPrice = 0,
        decimal maxPrice = 999999,
        List<string>? tags = null,
        Dictionary<string, List<string>>? properties = null,
        string searchPhrase = "",
        int limit = 10)
    {
        return await _client.PostAsJsonAsync("/api/v1/offers/search", new
        {
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Tags = tags ?? [],
            Properties = properties ?? [],
            SearchPhrase = searchPhrase,
            Limit = limit
        });
    }

    [Fact]
    public async Task Search_EmptyRequest_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/offers/search", "");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_NoOffers_ReturnsNoContent()
    {
        var response = await SearchOffers();

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Search_WithOffers_ReturnsOk()
    {
        await RegisterAndLogin("seller@example.com", "Password123!");
        var (category, mileageId, damagedId, _) = await GetCarsCategory();

        await CreateOfferInDb("seller@example.com", "BMW E46", 15000m, category, new()
        {
            [mileageId.ToString()] = "120000",
            [damagedId.ToString()] = "false"
        });

        var response = await SearchOffers();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Search_ByCategory_ReturnsOnlyMatchingCategory()
    {
        await RegisterAndLogin("seller2@example.com", "Password123!");
        var (carsCategory, mileageId, damagedId, _) = await GetCarsCategory();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var phonesCategory = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync(c => c.Name == "Phones");
        var storageId = phonesCategory.PropertyDefinitions.First(pd => pd.Name == "Storage GB").Id;
        var warrantyId = phonesCategory.PropertyDefinitions.First(pd => pd.Name == "Warranty").Id;

        await CreateOfferInDb("seller2@example.com", "BMW E46", 15000m, carsCategory, new()
        {
            [mileageId.ToString()] = "120000",
            [damagedId.ToString()] = "false"
        });

        await CreateOfferInDb("seller2@example.com", "iPhone 15", 4000m, phonesCategory, new()
        {
            [storageId.ToString()] = "256",
            [warrantyId.ToString()] = "true"
        });

        var response = await SearchOffers(categoryId: carsCategory.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(1);
    }

    [Fact]
    public async Task Search_ByPriceRange_ReturnsOnlyInRange()
    {
        await RegisterAndLogin("seller3@example.com", "Password123!");
        var (category, mileageId, damagedId, _) = await GetCarsCategory();

        await CreateOfferInDb("seller3@example.com", "Cheap car", 5000m, category, new()
        {
            [mileageId.ToString()] = "200000",
            [damagedId.ToString()] = "false"
        });

        await CreateOfferInDb("seller3@example.com", "Expensive car", 80000m, category, new()
        {
            [mileageId.ToString()] = "10000",
            [damagedId.ToString()] = "false"
        });

        var response = await SearchOffers(minPrice: 0, maxPrice: 10000);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(1);
    }

    [Fact]
    public async Task Search_MinPriceGreaterThanMaxPrice_ReturnsBadRequest()
    {
        var response = await SearchOffers(minPrice: 50000, maxPrice: 10000);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_InvalidCategoryId_ReturnsBadRequest()
    {
        var response = await SearchOffers(categoryId: 999);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_BySearchPhrase_ReturnsMatchingOffers()
    {
        await RegisterAndLogin("seller4@example.com", "Password123!");
        var (category, mileageId, damagedId, _) = await GetCarsCategory();

        await CreateOfferInDb("seller4@example.com", "BMW E46 Touring", 15000m, category, new()
        {
            [mileageId.ToString()] = "120000",
            [damagedId.ToString()] = "false"
        });

        await CreateOfferInDb("seller4@example.com", "Audi A4 Avant", 20000m, category, new()
        {
            [mileageId.ToString()] = "90000",
            [damagedId.ToString()] = "false"
        });

        var response = await SearchOffers(searchPhrase: "BMW");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body);
        json.RootElement.GetArrayLength().ShouldBe(1);
    }

    [Fact]
    public async Task Search_IsAnonymous_ReturnsOk()
    {
        ClearAuthHeader();
        await RegisterAndLogin("seller5@example.com", "Password123!");
        var (category, mileageId, damagedId, _) = await GetCarsCategory();

        await CreateOfferInDb("seller5@example.com", "Test car", 10000m, category, new()
        {
            [mileageId.ToString()] = "100000",
            [damagedId.ToString()] = "false"
        });

        ClearAuthHeader();
        var response = await SearchOffers();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Search_ComplexFiltering_ReturnsCorrectResults()
    {
        await RegisterAndLogin("complex@example.com", "Password123!");
        var (carsCategory, mileageId, damagedId, brandId) = await GetCarsCategory();

        await CreateOfferInDb("complex@example.com", "BMW E46 Cheap", 8000m, carsCategory, new()
        {
            [mileageId.ToString()] = "200000",
            [damagedId.ToString()] = "false",
            [brandId.ToString()] = "BMW"
        });

        await CreateOfferInDb("complex@example.com", "BMW M3 Expensive", 60000m, carsCategory, new()
        {
            [mileageId.ToString()] = "50000",
            [damagedId.ToString()] = "false",
            [brandId.ToString()] = "BMW"
        });

        await CreateOfferInDb("complex@example.com", "Audi A4 Mid", 25000m, carsCategory, new()
        {
            [mileageId.ToString()] = "120000",
            [damagedId.ToString()] = "false",
            [brandId.ToString()] = "Audi"
        });

        await CreateOfferInDb("complex@example.com", "Mercedes Damaged", 5000m, carsCategory, new()
        {
            [mileageId.ToString()] = "300000",
            [damagedId.ToString()] = "true",
            [brandId.ToString()] = "Mercedes"
        });

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var phonesCategory = await db.Set<Category>().Include(c => c.PropertyDefinitions).FirstAsync(c => c.Name == "Phones");
            var storageId = phonesCategory.PropertyDefinitions.First(pd => pd.Name == "Storage GB").Id;
            var warrantyId = phonesCategory.PropertyDefinitions.First(pd => pd.Name == "Warranty").Id;

            await CreateOfferInDb("complex@example.com", "iPhone 15", 4000m, phonesCategory, new()
            {
                [storageId.ToString()] = "256",
                [warrantyId.ToString()] = "true"
            });
        }

        var responseAll = await SearchOffers();
        responseAll.StatusCode.ShouldBe(HttpStatusCode.OK);
        var allOffers = JsonDocument.Parse(await responseAll.Content.ReadAsStringAsync());
        allOffers.RootElement.GetArrayLength().ShouldBe(5);

        var responseCars = await SearchOffers(categoryId: carsCategory.Id);
        responseCars.StatusCode.ShouldBe(HttpStatusCode.OK);
        var carsOffers = JsonDocument.Parse(await responseCars.Content.ReadAsStringAsync());
        carsOffers.RootElement.GetArrayLength().ShouldBe(4);

        var responseCheap = await SearchOffers(minPrice: 0, maxPrice: 10000);
        responseCheap.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cheapOffers = JsonDocument.Parse(await responseCheap.Content.ReadAsStringAsync());
        cheapOffers.RootElement.GetArrayLength().ShouldBe(3);

        var responseCheapCars = await SearchOffers(categoryId: carsCategory.Id, minPrice: 0, maxPrice: 10000);
        responseCheapCars.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cheapCarsOffers = JsonDocument.Parse(await responseCheapCars.Content.ReadAsStringAsync());
        cheapCarsOffers.RootElement.GetArrayLength().ShouldBe(2);

        var responseBmw = await SearchOffers(searchPhrase: "BMW");
        responseBmw.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bmwOffers = JsonDocument.Parse(await responseBmw.Content.ReadAsStringAsync());
        bmwOffers.RootElement.GetArrayLength().ShouldBe(2);

        var responseBmwBrand = await SearchOffers(
            categoryId: carsCategory.Id,
            properties: new Dictionary<string, List<string>>
            {
                [brandId.ToString()] = ["BMW"]
            });
        responseBmwBrand.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bmwBrandOffers = JsonDocument.Parse(await responseBmwBrand.Content.ReadAsStringAsync());
        bmwBrandOffers.RootElement.GetArrayLength().ShouldBe(2);

        var responseBmwAudiCheap = await SearchOffers(
            categoryId: carsCategory.Id,
            minPrice: 0,
            maxPrice: 30000,
            properties: new Dictionary<string, List<string>>
            {
                [brandId.ToString()] = ["BMW", "Audi"]
            });
        responseBmwAudiCheap.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bmwAudiCheapOffers = JsonDocument.Parse(await responseBmwAudiCheap.Content.ReadAsStringAsync());
        bmwAudiCheapOffers.RootElement.GetArrayLength().ShouldBe(2);

        var responseUndamaged = await SearchOffers(
            categoryId: carsCategory.Id,
            properties: new Dictionary<string, List<string>>
            {
                [damagedId.ToString()] = ["false"]
            });
        responseUndamaged.StatusCode.ShouldBe(HttpStatusCode.OK);
        var undamagedOffers = JsonDocument.Parse(await responseUndamaged.Content.ReadAsStringAsync());
        undamagedOffers.RootElement.GetArrayLength().ShouldBe(3);

        var responseNoMatch = await SearchOffers(minPrice: 999999, maxPrice: 999999);
        responseNoMatch.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}

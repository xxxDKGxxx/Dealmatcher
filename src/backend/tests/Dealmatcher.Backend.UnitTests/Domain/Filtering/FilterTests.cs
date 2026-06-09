using System.Linq.Expressions;
using Dealmatcher.Backend.Domain.Core.Filtering;
using Dealmatcher.Backend.Domain.Interfaces.Filtering;
using Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

namespace Dealmatcher.Backend.UnitTests.Domain.Filtering;

public class FilterTests
{
    private static Offer CreateOffer(
        decimal price = 1000m,
        string title = "Test",
        string description = "Description",
        Category? category = null,
        List<Property>? properties = null,
        List<string>? tags = null)
    {
        var cat = category ?? new Category("Cars", "Vehicles");
        var seller = new User("test@test.com", "hash", "Test", "User");
        return new Offer(title, description, price, [], seller, tags ?? [], 1, cat, properties ?? []);
    }

    private static Category CreateCarsCategory()
    {
        var category = new Category("Cars", "Vehicles")
        {
            Id = 1
        };
        return category;
    }

    // ── PriceFilter ──

    [Fact]
    public void PriceFilter_OffersInRange_Included()
    {
        var offers = new List<Offer>
        {
            CreateOffer(price: 500),
            CreateOffer(price: 1500),
            CreateOffer(price: 3000),
        };

        var filter = new PriceFilter(400, 2000);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(2);
    }

    [Fact]
    public void PriceFilter_ExactBoundaries_Included()
    {
        var offers = new List<Offer>
        {
            CreateOffer(price: 100),
            CreateOffer(price: 500),
        };

        var filter = new PriceFilter(100, 500);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(2);
    }

    [Fact]
    public void PriceFilter_NoMatch_Empty()
    {
        var offers = new List<Offer>
        {
            CreateOffer(price: 50),
            CreateOffer(price: 5000),
        };

        var filter = new PriceFilter(100, 500);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(0);
    }

    // ── CategoryFilter ──

    [Fact]
    public void CategoryFilter_MatchingCategory_Included()
    {
        var category1 = CreateCarsCategory();
        var category2 = new Category("Phones", "Devices")
        {
            Id = 2
        };

        var offers = new List<Offer>
        {
            CreateOffer(category: category1),
            CreateOffer(category: category2),
        };

        var filter = new CategoryFilter(1);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(1);
    }

    // ── SearchPhraseFilter ──

    [Fact]
    public void SearchPhraseFilter_MatchesTitle_Included()
    {
        var offers = new List<Offer>
        {
            CreateOffer(title: "BMW E46"),
            CreateOffer(title: "Audi A4"),
        };

        var filter = new SearchPhraseFilter("BMW");
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(1);
    }

    [Fact]
    public void SearchPhraseFilter_MatchesDescription_Included()
    {
        var offers = new List<Offer>
        {
            CreateOffer(description: "Great condition BMW"),
            CreateOffer(description: "Broken engine"),
        };

        var filter = new SearchPhraseFilter("BMW");
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(1);
    }

    [Fact]
    public void SearchPhraseFilter_EmptyPhrase_ReturnsAll()
    {
        var offers = new List<Offer>
        {
            CreateOffer(),
            CreateOffer(),
        };

        var filter = new SearchPhraseFilter("");
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(2);
    }

    // ── TagFilter ──

    [Fact]
    public void TagFilter_MatchingTag_Included()
    {
        var offers = new List<Offer>
        {
            CreateOffer(tags: ["used", "cheap"]),
            CreateOffer(tags: ["new"]),
        };

        var filter = new TagFilter(["used"]);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(1);
    }

    [Fact]
    public void TagFilter_MultipleTagsAnyMatch_Included()
    {
        var offers = new List<Offer>
        {
            CreateOffer(tags: ["used"]),
            CreateOffer(tags: ["new"]),
            CreateOffer(tags: ["broken"]),
        };

        var filter = new TagFilter(["used", "new"]);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(2);
    }

    // ── BooleanPropertyFilter ──

    [Fact]
    public void BooleanPropertyFilter_MatchingValue_Included()
    {
        var definition = new BooleanPropertyDefinition("Damaged", PropertyType.Boolean)
        {
            Id = 10
        };

        var offers = new List<Offer>
        {
            CreateOffer(properties: [new BooleanProperty(definition, true)]),
            CreateOffer(properties: [new BooleanProperty(definition, false)]),
        };

        var filter = new BooleanPropertyFilter(definition, false);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(1);
    }

    // ── NumericPropertyFilter ──

    [Fact]
    public void NumericPropertyFilter_InRange_Included()
    {
        var definition = new NumericPropertyDefinition("Mileage", PropertyType.Number)
        {
            Id = 20
        };

        var offers = new List<Offer>
        {
            CreateOffer(properties: [new NumericProperty(definition, 50000)]),
            CreateOffer(properties: [new NumericProperty(definition, 150000)]),
            CreateOffer(properties: [new NumericProperty(definition, 250000)]),
        };

        var filter = new NumericPropertyFilter(definition, 0, 200000);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(2);
    }

    // ── SelectPropertyFilter ──

    [Fact]
    public void SelectPropertyFilter_MatchingValue_Included()
    {
        var definition = new SelectPropertyDefinition("Brand", PropertyType.Select, ["BMW", "Audi", "Mercedes"])
        {
            Id = 30
        };

        var offers = new List<Offer>
        {
            CreateOffer(properties: [new SelectProperty(definition, "BMW")]),
            CreateOffer(properties: [new SelectProperty(definition, "Audi")]),
            CreateOffer(properties: [new SelectProperty(definition, "Mercedes")]),
        };

        var filter = new SelectPropertyFilter(definition, ["BMW", "Audi"]);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(2);
    }

    // ── TextPropertyFilter ──

    [Fact]
    public void TextPropertyFilter_MatchingValue_Included()
    {
        var definition = new TextPropertyDefinition("Brand", PropertyType.Text)
        {
            Id = 40
        };

        var offers = new List<Offer>
        {
            CreateOffer(properties: [new TextProperty(definition, "Nike")]),
            CreateOffer(properties: [new TextProperty(definition, "Adidas")]),
        };

        var filter = new TextPropertyFilter(definition, ["Nike"]);
        var result = ApplyFilter(offers, filter);

        result.Count.ShouldBe(1);
    }

    // ── FilteredOffersSpecification (combined) ──

    [Fact]
    public void CombinedFilters_AllApplied()
    {
        var category = CreateCarsCategory();
        var mileageDef = new NumericPropertyDefinition("Mileage", PropertyType.Number)
        {
            Id = 20
        };

        var offers = new List<Offer>
        {
            CreateOffer(price: 5000, category: category, properties: [new NumericProperty(mileageDef, 50000)], tags: ["used"]),
            CreateOffer(price: 50000, category: category, properties: [new NumericProperty(mileageDef, 150000)], tags: ["used"]),
            CreateOffer(price: 5000, category: category, properties: [new NumericProperty(mileageDef, 300000)], tags: ["used"]),
        };

        var filters = new List<IFilter>
        {
            new PriceFilter(0, 10000),
            new NumericPropertyFilter(mileageDef, 0, 200000),
        };

        var spec = new FilteredOffersSpecification(filters);
        var result = offers.AsQueryable().Where(spec.WhereExpressions.Aggregate(
            (Expression<Func<Offer, bool>>)(o => true),
            (current, next) => Expression.Lambda<Func<Offer, bool>>(
                Expression.AndAlso(current.Body, Expression.Invoke(next.Filter, current.Parameters)),
                current.Parameters)));

        result.Count().ShouldBe(1);
    }

    // ── Helper ──

    private static List<Offer> ApplyFilter(List<Offer> offers, IFilter filter)
    {
        var spec = new FilteredOffersSpecification([filter]);
        return [.. offers.Where(o =>
        {
            foreach (var expr in spec.WhereExpressions)
            {
                var compiled = expr.Filter.Compile();
                if (!compiled(o)) return false;
            }
            return true;
        })];
    }
}

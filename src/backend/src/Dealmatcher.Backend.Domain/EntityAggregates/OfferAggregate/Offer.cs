namespace Dealmatcher.Backend.Domain.EntityAggregates.OfferAggregate;

public sealed class Offer : DealmatcherEntityBase, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    private readonly List<string> _images = [];
    public IReadOnlyCollection<string> Images => _images.AsReadOnly();
    public User Seller { get; private set; } = null!;
    public OfferStatus Status { get; private set; } = null!;
    private readonly List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
    public int Availability { get; private set; }
    public Category Category { get; private set; } = null!;
    private readonly List<Property> _properties = [];
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

    public Offer(
        string title,
        string description,
        decimal price,
        List<string> images,
        List<string> tags,
        int availability,
        Category category,
        List<Property> properties)
    {
        Title = title;
        Description = description;
        Price = price;
        _images = images;
        Status = OfferStatus.Active;
        _tags = tags;
        Availability = availability;
        Category = category;
        _properties = properties;
    }

    private Offer() { }

    public override void Delete()
    {
        Status = OfferStatus.Deleted;
        foreach (var property in _properties)
        {
            property.Delete();
        }
    }

    public void UpdateTitle(string title)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdatePrice(decimal price)
    {
        if (price >= 0)
        {
            Price = price;
        }
    }

    public void Sell(int amount)
    {
        if (Status.CanBeSold && Availability >= amount)
        {
            Status = OfferStatus.Sold;
        }
    }

    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            _tags.Add(tag);
        }
    }

    public void RemoveTag(string tag)
    {
        _tags.Remove(tag);
    }

    public void SetTags(IEnumerable<string> tags)
    {
        _tags.Clear();
        _tags.AddRange(tags.Where(t => !string.IsNullOrWhiteSpace(t)));
    }

    public void AddImage(string image)
    {
        if (!string.IsNullOrWhiteSpace(image))
        {
            _images.Add(image);
        }
    }

    public void RemoveImage(string image)
    {
        _images.Remove(image);
    }

    public void SetImages(IEnumerable<string> images)
    {
        _images.Clear();
        _images.AddRange(images.Where(i => !string.IsNullOrWhiteSpace(i)));
    }

    public void SetAvailability(int availability)
    {
        if (availability >= 0)
        {
            Availability = availability;
        }
    }
    public void SetProperties(IEnumerable<Property> properties)
    {
        _properties.Clear();
        _properties.AddRange(properties);
    }
    public void SetCategory(Category category, IEnumerable<Property> properties)
    {
        Category = category;
        SetProperties(properties);
    }
}

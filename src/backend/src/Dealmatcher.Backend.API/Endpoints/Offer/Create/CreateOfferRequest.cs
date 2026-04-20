namespace Dealmatcher.Backend.API.Endpoints.Offer.Create;

public sealed class CreateOfferRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public IFormFileCollection? Images { get; set; }
    public int CategoryId { get; set; }
    public List<string> Tags { get; set; } = [];
    public Dictionary<string, string> Properties { get; set; } = [];
    public int Availability { get; set; } = 1;
}

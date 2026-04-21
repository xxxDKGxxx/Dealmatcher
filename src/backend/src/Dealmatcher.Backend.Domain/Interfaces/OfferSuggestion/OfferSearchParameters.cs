namespace Dealmatcher.Backend.Domain.Interfaces.OfferSuggestion;

// TODO: dodać listę/słownik filtrów
public sealed record OfferSearchParameters(
    int? CategoryId,
    decimal MinPrice,
    decimal MaxPrice,
    List<string> Tags,
    string SearchPhrase,
    int Limit);

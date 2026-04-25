namespace Dealmatcher.Backend.Domain.Core.Cart;

public sealed class Cart(int userId)
{
    public int UserId { get; init; } = userId;
    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public void IncludeItemInQuantity(int offerId, int quantity)
    {
        _items.RemoveAll(i => i.OfferId == offerId);
        _items.Add(new CartItem(offerId, quantity));
    }

    public void DeleteItem(int offerId)
    {
        _items.RemoveAll(i => i.OfferId == offerId);
    }
}

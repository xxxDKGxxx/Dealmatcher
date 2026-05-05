namespace Dealmatcher.Backend.Domain.Core.Cart;

public sealed class Cart(int userId)
{
    public int UserId { get; init; } = userId;
    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public void UpdateItemQuantity(int offerId, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Quantity of an item must be non negative");
        }

        if (quantity == 0)
        {
            RemoveItem(offerId);
            return;
        }

        _items.RemoveAll(i => i.OfferId == offerId);
        _items.Add(new CartItem(offerId, quantity, DateTime.Now));
    }

    public void RemoveItem(int offerId)
    {
        _items.RemoveAll(i => i.OfferId == offerId);
    }

    public void Clear()
    {
        _items.Clear();
    }
}

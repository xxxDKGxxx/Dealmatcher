namespace Dealmatcher.Backend.API.Endpoints.Cart.Update;

public sealed record UpdateItemRequest(int CartItemId, int Quantity);

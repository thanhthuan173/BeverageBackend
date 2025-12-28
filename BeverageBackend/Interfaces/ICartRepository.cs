using BeverageBackend.Models;

namespace BeverageBackend.Interfaces
{
    public interface ICartRepository
    {
        ICollection<Cart> GetCarts();
        Cart GetCart(int id);
        Customer GetCustomerByCartId(int cartId);
        ICollection<CartItem> GetCartItems(int cartId);
        bool CartExists(int id);
    }
}

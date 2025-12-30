using BeverageBackend.Interfaces;
using BeverageBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BeverageBackend.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly BeverageDbContext _context;

        public CartRepository(BeverageDbContext context)
        {
            _context = context;
        }

        public bool CartExists(int id)
        {
            return _context.Carts.Any(c => c.Id == id);
        }

        public Cart GetCart(int id)
        {
            return _context.Carts.Where(c => c.Id == id).Include(c=>c.CartItems).ThenInclude(ci=>ci.Product).FirstOrDefault();
        }

        public ICollection<Cart> GetCarts()
        {
            return _context.Carts.ToList();
        }

        public Customer GetCustomerByCartId(int cartId)
        {
            return _context.Carts.Where(c => c.Id == cartId).Select(c => c.Customer).FirstOrDefault();
        }

        public ICollection<CartItem> GetCartItems(int cartId)
        {
            return _context.CartItems.Where(ci => ci.CartId == cartId).Include(ci=>ci.Product).ToList();
        }
    }
}

using BeverageBackend.Interfaces;
using BeverageBackend.Models;

namespace BeverageBackend.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly BeverageDbContext _context;
        public ProductRepository(BeverageDbContext context)
        {
            _context = context;
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Where(p => p.Id == id).FirstOrDefault();
        }

        public Product GetProduct(string name)
        {
            var qr = from prod in _context.Products
                     where prod.Name.Equals(name)
                     select prod;
            return qr.FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id==id);
        }

        public int ProductOrders(int id)
        {
            var order = _context.OrderItems.Where(oi => oi.ProductId == id);
            return order.Count();
        }
    }
}

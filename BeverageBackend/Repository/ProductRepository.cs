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
        public ICollection<Product> GetProducts()
        {
            return _context.Products.ToList();
        }
    }
}

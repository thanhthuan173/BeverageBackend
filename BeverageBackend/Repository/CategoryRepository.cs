using BeverageBackend.Interfaces;
using BeverageBackend.Models;

namespace BeverageBackend.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private BeverageDbContext _context;

        public CategoryRepository(BeverageDbContext context)
        {
            _context = context;
        }

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.Id).ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        }

        public ICollection<Product> GetProductsByCategory(int id)
        {
            var p = _context.Products.Where(p => p.CategoryId == id).ToList();
            return p;
        }
    }
}

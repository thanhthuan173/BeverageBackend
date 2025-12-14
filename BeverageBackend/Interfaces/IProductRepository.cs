using BeverageBackend.Models;

namespace BeverageBackend.Interfaces
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
    }
}

using BeverageBackend.Models;

namespace BeverageBackend.Interfaces
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int id);
        Product GetProduct(string name);
        int ProductOrders(int id);
        bool ProductExists(int id);
    }
}

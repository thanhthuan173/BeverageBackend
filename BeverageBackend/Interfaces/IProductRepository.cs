using BeverageBackend.Models;

namespace BeverageBackend.Interfaces
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int id);
        Product GetProduct(string name);
        int CountOrders(int prodId);
        int CountCarts(int prodId);
        bool ProductExists(int id);
    }
}

using BeverageBackend.Models;

namespace BeverageBackend.Interfaces
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int id);
        Product GetProduct(string name);
        int CountOrders(int prodId);
        ICollection<Product> GetProductsOfAOrder(int orderId);
        ICollection<Order> GetOrdersByProduct(int prodId);
        bool ProductExists(int id);
    }
}

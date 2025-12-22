using BeverageBackend.Models;

namespace BeverageBackend.Interfaces
{
    public interface ICustomerRepository
    {
        ICollection<Customer> GetCustomers();
        Customer GetCustomer(int id);
        ICollection<Order> GetOrdersByCustomer(int id);
        bool CustomerExits(int id);
    }
}

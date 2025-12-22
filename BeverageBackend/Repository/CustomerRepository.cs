using BeverageBackend.Interfaces;
using BeverageBackend.Models;

namespace BeverageBackend.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private BeverageDbContext _context;

        public CustomerRepository(BeverageDbContext context)
        {
            _context = context;
        }
        public bool CustomerExits(int id)
        {
            return _context.Customers.Any(c => c.Id == id);
        }

        public Customer GetCustomer(int id)
        {
            return _context.Customers.Where(c => c.Id == id).FirstOrDefault();
        }

        public ICollection<Customer> GetCustomers()
        {
            return _context.Customers.OrderBy(c=>c.Id).ToList();
        }

        public ICollection<Order> GetOrdersByCustomer(int id)
        {
            var qr = _context.Orders.Where(o => o.CustomerId == id);
            return qr.ToList();
        }
    }
}

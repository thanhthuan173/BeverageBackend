using BeverageBackend.Interfaces;
using BeverageBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BeverageBackend.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BeverageDbContext _context;

        public OrderRepository(BeverageDbContext context)
        {
            _context = context;
        }

        public Customer GetCustomerByOrderId(int orderId)
        {
            return _context.Orders.Where(o => o.Id == orderId).Select(o => o.Customer).FirstOrDefault();
        }

        public Order GetOrder(int id)
        {
            return _context.Orders.Where(o => o.Id == id).Include(o => o.OrderItems).ThenInclude(oi=>oi.Product).FirstOrDefault();
        }

        public ICollection<Order> GetOrders()
        {
            return _context.Orders.ToList();
        }

        public bool OrderExists(int orderId)
        {
            return _context.Orders.Any(o => o.Id == orderId);
        }
    }
}

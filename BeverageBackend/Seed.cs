using System;
using System.Collections.Generic;
using System.Linq;
using BeverageBackend.Models;

namespace BeverageBackend
{
    public class Seed
    {
        private readonly BeverageDbContext _context;

        public Seed(BeverageDbContext context)
        {
            _context = context;
        }

        public void SeedData()
        {
            if (_context.Products.Any()) return;

            //CATEGORIES
            var categories = new List<Category>()
            {
                new Category { Name = "Coffee" },
                new Category { Name = "Tea" },
                new Category { Name = "Smoothie" }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            //PRODUCTS
            var products = new List<Product>()
            {
                new Product {
                    Name = "Black Coffee",
                    Description = "Strong black coffee",
                    Price = 30000,
                    Stock = 50,
                    ImgUrl = "img/coffee1.jpg",
                    CategoryId = categories[0].Id
                },
                new Product {
                    Name = "Milk Tea",
                    Description = "Sweet milk tea",
                    Price = 35000,
                    Stock = 60,
                    ImgUrl = "img/tea1.jpg",
                    CategoryId = categories[1].Id
                },
                new Product {
                    Name = "Strawberry Smoothie",
                    Description = "Fresh strawberry smoothie",
                    Price = 45000,
                    Stock = 40,
                    ImgUrl = "img/smoothie1.jpg",
                    CategoryId = categories[2].Id
                }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();

            //CUSTOMERS
            var customers = new List<Customer>()
            {
                new Customer {
                    FullName = "Nguyen Van A",
                    Gender = "Male",
                    Phone = "0909000001",
                    Email = "a@example.com",
                    Address = "123 Street",
                    Username = "nguyenvana",
                    HashPassword = "123456"  //hash
                },
                new Customer {
                    FullName = "Tran Thi B",
                    Gender = "Female",
                    Phone = "0909000002",
                    Email = "b@example.com",
                    Address = "456 Street",
                    Username = "tranthib",
                    HashPassword = "123456"
                }
            };

            _context.Customers.AddRange(customers);
            _context.SaveChanges();

            //CARTS
            var carts = new List<Cart>()
            {
                new Cart { CustomerId = customers[0].Id },
                new Cart { CustomerId = customers[1].Id }
            };

            _context.Carts.AddRange(carts);
            _context.SaveChanges();

            //CARTITEMS
            var cartItems = new List<CartItem>()
            {
                new CartItem {
                    CartId = carts[0].Id,
                    ProductId = products[0].Id,
                    Quantity = 2,
                    UnitPrice = products[0].Price
                },
                new CartItem {
                    CartId = carts[1].Id,
                    ProductId = products[1].Id,
                    Quantity = 1,
                    UnitPrice = products[1].Price
                }
            };

            _context.CartItems.AddRange(cartItems);
            _context.SaveChanges();

            //ORDERS
            var orders = new List<Order>()
            {
                new Order {
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Status = "Completed",
                    TotalAmount = 60000,
                    CustomerId = customers[0].Id
                },
                new Order {
                    CreatedDate = DateTime.Now,
                    Status = "Pending",
                    TotalAmount = 35000,
                    CustomerId = customers[1].Id
                }
            };

            _context.Orders.AddRange(orders);
            _context.SaveChanges();

            //ORDERITEMS
            var orderItems = new List<OrderItem>()
            {
                new OrderItem {
                    OrderId = orders[0].Id,
                    ProductId = products[0].Id,
                    Quantity = 2,
                    UnitPrice = products[0].Price
                },
                new OrderItem {
                    OrderId = orders[1].Id,
                    ProductId = products[1].Id,
                    Quantity = 1,
                    UnitPrice = products[1].Price
                }
            };

            _context.OrderItems.AddRange(orderItems);
            _context.SaveChanges();
        }
    }
}

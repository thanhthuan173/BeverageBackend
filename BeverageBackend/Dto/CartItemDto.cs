using BeverageBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeverageBackend.Dto
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

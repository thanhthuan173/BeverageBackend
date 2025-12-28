using System.ComponentModel.DataAnnotations.Schema;

namespace BeverageBackend.Dto
{
    public class OrderItemDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace BeverageBackend.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }

        public int CustomerId { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }
}

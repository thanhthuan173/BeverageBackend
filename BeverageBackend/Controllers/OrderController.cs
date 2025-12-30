using AutoMapper;
using BeverageBackend.Dto;
using BeverageBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeverageBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OrderController:ControllerBase
    {
        private readonly IOrderRepository _order;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository order,IMapper mapper)
        {
            _order = order;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var oders = _mapper.Map<List<OrderDto>>(_order.GetOrders());
            return Ok(oders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOder(int id)
        {
            var order = _mapper.Map<OrderDto>(_order.GetOrder(id));
            return Ok(order);
        }

        [HttpGet("{orderId}/customer")]
        public IActionResult GetCustomerByOrderId(int orderId)
        {
            var cus = _mapper.Map<CustomerDto>(_order.GetCustomerByOrderId(orderId));
            return Ok(cus);
        }
    }
}

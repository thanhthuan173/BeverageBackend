using AutoMapper;
using BeverageBackend.Dto;
using BeverageBackend.Interfaces;
using BeverageBackend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BeverageBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CustomerController:ControllerBase
    {
        private ICustomerRepository _customer;
        private IMapper _mapper;

        public CustomerController(ICustomerRepository customer,IMapper mapper)
        {
            _customer = customer;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllCustomer()
        {
            var customers= _mapper.Map<List<CustomerDto>>(_customer.GetCustomers());
            return Ok(customers);
        }
            

        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            if (_customer.CustomerExits(id))
                NotFound();
            var cus = _mapper.Map<CustomerDto>(_customer.GetCustomer(id));
            return Ok(cus);
        }

        [HttpGet("{id}/orders")]
        public IActionResult GetOrders(int id)
        {
            if (_customer.CustomerExits(id))
                NotFound();
            var orders = _customer.GetOrdersByCustomer(id);
            return Ok(orders);
        }
    }
}

using AutoMapper;
using BeverageBackend.Dto;
using BeverageBackend.Interfaces;
using BeverageBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeverageBackend.Controllers
{ 
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductController:ControllerBase
    {
        private readonly IProductRepository _product;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository,IMapper mapper)
        {
            _product = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllProduct()
        {
            var prod = _mapper.Map<List<ProductDto>>(_product.GetProducts().ToList());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(prod);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProductById(int id)
        {
            if (!_product.ProductExists(id))
                return NotFound();
            var prod = _mapper.Map<ProductDto>(_product.GetProduct(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(prod);
        }

        [HttpGet("{name}")]
        public IActionResult GetProductByName(string name)
        {
            var prod = _mapper.Map<ProductDto>(_product.GetProduct(name));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(prod);
        }

        [HttpGet("{prodId}/count_orders")]
        public IActionResult ProductOrders(int prodId)
        {
            if (!_product.ProductExists(prodId))
                return NotFound();
            return Ok(_product.CountOrders(prodId));
        }

        [HttpGet("{prodId}/find_orders")]
        public IActionResult GetOrdersByProduct(int prodId)
        {
            if (!_product.ProductExists(prodId))
                return NotFound();
            return Ok(_product.GetOrdersByProduct(prodId));
        }

        [HttpGet("{orderId}/find_products")]
        public IActionResult GetProductsOfAOrder(int orderId)
        {
            return Ok(_product.GetProductsOfAOrder(orderId));
        }
    }
}

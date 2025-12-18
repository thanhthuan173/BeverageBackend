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

        [HttpGet("{id}/orders")]
        public IActionResult ProductOrders(int id)
        {
            if (!_product.ProductExists(id))
                return NotFound();
            return Ok(_product.ProductOrders(id));
        }
    }
}

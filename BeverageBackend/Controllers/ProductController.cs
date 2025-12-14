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
        public ProductController(IProductRepository productRepository)
        {
            _product = productRepository;
        }

        [HttpGet]
        public IActionResult GetAllProduct()
        {
            var prod = _product.GetProducts().ToList();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(prod);
        }
    }
}

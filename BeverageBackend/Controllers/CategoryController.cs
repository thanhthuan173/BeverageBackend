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
    public class CategoryController:ControllerBase
    {
        private ICategoryRepository _category;
        private IMapper _mapper;

        public CategoryController(ICategoryRepository category,IMapper mapper)
        {
            _category = category;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var cates = _mapper.Map<List<CategoryDto>>(_category.GetCategories());
            return Ok(cates);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            if (!_category.CategoryExists(id))
                return NotFound();
            var cate = _mapper.Map<CategoryDto>(_category.GetCategory(id));
            return Ok(cate);
        }

        [HttpGet("{id}/products")]
        public IActionResult GetAllProductByCateId(int id)
        {
            if (!_category.CategoryExists(id))
                return NotFound();
            var prods = _mapper.Map<List<ProductDto>>(_category.GetProductsByCategory(id));
            return Ok(prods);
        }
    }
}

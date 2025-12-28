using AutoMapper;
using BeverageBackend.Dto;
using BeverageBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeverageBackend.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CartController:ControllerBase
    {
        private readonly ICartRepository _cart;
        private readonly IMapper _mapper;

        public CartController(ICartRepository cart,IMapper mapper)
        {
            _cart = cart;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllCarts()
        {
            var carts = _mapper.Map<List<CartDto>>(_cart.GetCarts());
            return Ok(carts);
        }

        [HttpGet("{id}")]
        public IActionResult GetCart(int id)
        {
            if (!_cart.CartExists(id))
                return NotFound();
            var cart = _mapper.Map<CartDto>(_cart.GetCart(id));
            return Ok(cart);
        }

        [HttpGet("{cartId}/customer")]
        public IActionResult GetCustomerByCartId(int cartId)
        {
            if (!_cart.CartExists(cartId))
                return NotFound();
            var cus = _mapper.Map<CustomerDto>(_cart.GetCustomerByCartId(cartId));
            return Ok(cus);
        }

        [HttpGet("{cartId}/items")]
        public IActionResult GetCartItems(int cartId)
        {
            if (!_cart.CartExists(cartId))
                return NotFound();
            var prods = _mapper.Map<List<CartItemDto>>(_cart.GetCartItems(cartId));
            return Ok(prods);
        }
    }
}

using AutoMapper;
using EcommerceBackend.Application.Dto.Cart;
using EcommerceBackend.Application.Exceptions;
using EcommerceBackend.Application.Interfaces;
using EcommerceBackend.Application.Interfaces.Services;
using EcommerceBackend.Application.Services;
using EcommerceBackend.Domain.Models;
using Moq;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly ICartService _cartService;

    public CartServiceTests()
    {
        _cartRepositoryMock = new Mock<ICartRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _cartService = new CartService
        (
            _cartRepositoryMock.Object,
            _productRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _currentUserServiceMock.Object
        );
    }

    #region Add To Cart
    [Fact]
    public async Task AddToCartAsync_CartNotFound_ThrowNotFoundException()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync((Cart?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _cartService.AddToCartAsync(new AddCartItemDto()));
    }

    [Fact]
    public async Task AddToCartAsync_ProductNotFound_ThrowNotFoundException()
    {
        var cart = CreateCart();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _cartService.AddToCartAsync(new AddCartItemDto()));
    }

    [Fact]
    public async Task AddToCartAsync_InvalidQuantity_ThrowBadRequestException()
    {
        var cart = CreateCart();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Product()
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            Stock = 5
        });

        await Assert.ThrowsAsync<BadRequestException>(() => _cartService.AddToCartAsync(new AddCartItemDto
        {
            ProductId = 1,
            Quantity = 0
        }));
    }

    [Fact]
    public async Task AddToCartAsync_ItemExistsAndInvalidQuantity_ThrowBadRequestException()
    {
        var cart = CreateCart();
        var product = new Product
        {
            Id = 1,
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            Stock = 4
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

        await Assert.ThrowsAsync<BadRequestException>(() => _cartService.AddToCartAsync(new AddCartItemDto
        {
            ProductId = 1,
            Quantity = 4,
        }));
    }
    #endregion

    #region Get Cart
    [Fact]
    public async Task GetCartAsync_CartIsNull_ThrowNotFoundException()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync((Cart?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _cartService.GetCartAsync());
    }

    [Fact]
    public async Task GetCartAsync_GetSuccessfully_ReturnCartDto()
    {
        var cart = new Cart
        {
            CartItems = new List<CartItem>()
            {
                new CartItem
                {
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice = 10
                },
                new CartItem
                {
                    ProductId=2,
                    Quantity=3,
                    UnitPrice=5
                }
            }
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync(cart);
        _mapperMock.Setup(x => x.Map<CartDto>(cart)).Returns(new CartDto()
        {
            CartItems = cart.CartItems.Select(x => new CartItemDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductId.ToString(),
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        });

        var result = await _cartService.GetCartAsync();

        Assert.Equal(35, result.TotalAmount);
        Assert.Equal(2, result.CartItems.Count);
    }
    #endregion

    #region Remove Item
    [Fact]
    public async Task RemoveItemAsync_CartNotFound_ThrowNotFoundException()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync((Cart?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _cartService.RemoveItemAsync(1));
    }
    [Fact]
    public async Task RemoveItemAsync_ItemNotFound_ThrowNotFoundException()
    {
        var cart = CreateCart();
        await Assert.ThrowsAsync<NotFoundException>(() => _cartService.RemoveItemAsync(3));
    }

    [Fact]
    public async Task RemoveItemAsync_ItemExists_RemoveSuccessfully()
    {
        var cart = CreateCart();

        await _cartService.RemoveItemAsync(1);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        Assert.Empty(cart.CartItems);
    }
    #endregion

    #region Clear Cart
    [Fact]
    public async Task ClearCartAsync_CartNotFound_ThrowNotFoundException()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync((Cart?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _cartService.ClearCartAsync());
    }
    [Fact]
    public async Task ClearCartAsync_ClearSuccessfully_CartItemsIsEmpty()
    {
        var cart = CreateCart();

        await _cartService.ClearCartAsync();

        Assert.Empty(cart.CartItems);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    #endregion

    private Cart CreateCart()
    {
        var cart = new Cart()
        {
            CartItems = new List<CartItem>()
            {
                new CartItem
                {
                    ProductId = 1,
                    Quantity = 2,
                    UnitPrice=10
                }
            }
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync(cart);
        return cart;
    }
}
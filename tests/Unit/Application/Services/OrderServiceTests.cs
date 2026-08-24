using AutoMapper;
using EcommerceBackend.Application.Common;
using EcommerceBackend.Application.Common.Query;
using EcommerceBackend.Application.Dto.Order;
using EcommerceBackend.Application.Exceptions;
using EcommerceBackend.Application.Interfaces;
using EcommerceBackend.Application.Interfaces.Services;
using EcommerceBackend.Application.Services;
using EcommerceBackend.Domain.Enums;
using EcommerceBackend.Domain.Models;
using Moq;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _cartRepositoryMock = new Mock<ICartRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _orderService = new OrderService
        (
            _orderRepositoryMock.Object,
            _cartRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _currentUserServiceMock.Object
        );
    }

    #region Get Orders
    [Fact]
    public async Task GetAllOrdersAsync_ReturnPagedOrderDtos()
    {
        var query = new OrderQueryParameters
        {
            PageSize = 10,
            PageNumber = 1
        };
        var orders = new List<Order>()
        {
            new Order
            {
                Id=1,
                Status = OrderStatus.PendingPayment,
                TotalAmount = 10
            },
            new Order
            {
                Id=2,
                Status=OrderStatus.Completed,
                TotalAmount=20
            }
        };
        var orderDtos = new List<OrderDto>()
        {
            new OrderDto
            {
                Id=1,
                Status = OrderStatus.PendingPayment,
                TotalAmount = 10
            },
            new OrderDto
            {
                Id=2,
                Status=OrderStatus.Completed,
                TotalAmount=20
            }
        };
        var pagedResult = new PagedResult<Order>(orders, 2, 1, 10);
        _orderRepositoryMock.Setup(x => x.GetAllOrdersAsync(query)).ReturnsAsync(pagedResult);
        _mapperMock.Setup(x => x.Map<List<OrderDto>>(orders)).Returns(orderDtos);

        var result = await _orderService.GetAllOrdersAsync(query);

        Assert.Equal(query.PageNumber, result.PageNumber);
        Assert.Equal(query.PageSize, result.PageSize);
        Assert.Equal(orderDtos, result.Items);
        _orderRepositoryMock.Verify(x => x.GetAllOrdersAsync(query), Times.Once);
    }

    [Fact]
    public async Task GetMyOrdersAsync_ReturnPagedOrderDtos()
    {
        var query = new OrderQueryParameters
        {
            PageSize = 10,
            PageNumber = 1
        };
        var orders = new List<Order>()
        {
            new Order
            {
                Id=1,
                Status = OrderStatus.PendingPayment,
                TotalAmount = 10
            },
            new Order
            {
                Id=2,
                Status=OrderStatus.Completed,
                TotalAmount=20
            }
        };
        var orderDtos = new List<OrderDto>()
        {
            new OrderDto
            {
                Id=1,
                Status = OrderStatus.PendingPayment,
                TotalAmount = 10
            },
            new OrderDto
            {
                Id=2,
                Status=OrderStatus.Completed,
                TotalAmount=20
            }
        };
        var pagedResult = new PagedResult<Order>(orders, 2, 1, 10);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _orderRepositoryMock.Setup(x => x.GetByUserAsync(1, query)).ReturnsAsync(pagedResult);
        _mapperMock.Setup(x => x.Map<List<OrderDto>>(orders)).Returns(orderDtos);

        var result = await _orderService.GetMyOrdersAsync(query);

        Assert.Equal(orderDtos, result.Items);
        Assert.Equal(query.PageNumber, result.PageNumber);
        Assert.Equal(query.PageSize, result.PageSize);
        _orderRepositoryMock.Verify(x => x.GetByUserAsync(1, query), Times.Once);
    }
    #endregion

    #region Get Order
    [Fact]
    public async Task GetOrderByIdAsync_OrderDoesNotExist_ThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(1, false)).ReturnsAsync((Order?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetOrderByIdAsync(1));
    }

    [Fact]
    public async Task GetOrderByIdAsync_InvalidUser_ThrowForbiddenException()
    {
        var order = new Order()
        {
            UserId = 1
        };
        _orderRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(1, false)).ReturnsAsync(order);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(2);

        await Assert.ThrowsAsync<ForbiddenException>(() => _orderService.GetOrderByIdAsync(1));
    }

    [Fact]
    public async Task GetOrderByIdAsync_OrderExists_ReturnOrderDetailDto()
    {
        var order = new Order()
        {
            Status = OrderStatus.PendingPayment,
            UserId = 1
        };
        var orderDetailDto = new OrderDetailDto()
        {
            Status = OrderStatus.PendingPayment,
            UserId = 1
        };
        _orderRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(1, false)).ReturnsAsync(order);
        _mapperMock.Setup(x => x.Map<OrderDetailDto>(order)).Returns(orderDetailDto);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);

        var result = await _orderService.GetOrderByIdAsync(1);

        Assert.Equal(order.UserId, orderDetailDto.UserId);
        Assert.Equal(order.Status, orderDetailDto.Status);
        _orderRepositoryMock.Verify(x => x.GetByIdWithItemsAsync(1, false), Times.Once);
    }

    [Fact]
    public async Task GetOrderByIdForAdminAsync_OrderDoesNotExist_ThrowNotFoundException()
    {
        _orderRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(1, true)).ReturnsAsync((Order?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetOrderByIdForAdminAsync(1));
    }

    [Fact]
    public async Task GetOrderByIdForAdminAsync_OrderExists_ReturnOrderDetailDto()
    {
        var order = new Order
        {
            Status = OrderStatus.PendingPayment
        };
        var orderDetailDto = new OrderDetailDto
        {
            Status = OrderStatus.PendingPayment
        };
        _orderRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(1, true)).ReturnsAsync(order);
        _mapperMock.Setup(x => x.Map<OrderDetailDto>(order)).Returns(orderDetailDto);

        var result = await _orderService.GetOrderByIdForAdminAsync(1);

        Assert.Equal(order.Status, result.Status);
        _orderRepositoryMock.Verify(x => x.GetByIdWithItemsAsync(1, true), Times.Once);
    }
    #endregion

    #region Create Order
    [Fact]
    public async Task CreateOrderAsync_CartDoesNotExist_ThrowNotFoundException()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync((Cart?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _orderService.CreateOrderAsync());
    }

    [Fact]
    public async Task CreateOrderAsync_CartIsEmpty_ThrowBadRequestException()
    {
        var cart = new Cart
        {
            Id = 1,
            CartItems = []
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync(cart);
        await Assert.ThrowsAsync<BadRequestException>(() => _orderService.CreateOrderAsync());
    }

    [Fact]
    public async Task CreateOrderAsync_CartItemQuantityLessThanProductStock_ThrowBadRequestException()
    {
        var cart = new Cart
        {
            CartItems = new List<CartItem>()
            {
                new CartItem
                {
                    Quantity = 3,
                    Product = new Product
                    {
                        Name = "Product 1",
                        ImgUrl = "imgs/prodImg.jpg",
                        Stock = 2
                    }
                }
            }
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync(cart);
        await Assert.ThrowsAsync<BadRequestException>(() => _orderService.CreateOrderAsync());
    }

    [Fact]
    public async Task CreateOrderAsync_CreateSuccessfully_ReturnOrderDetailDto()
    {
        var cart = new Cart()
        {
            CartItems = new List<CartItem>()
            {
                new CartItem
                {
                    Quantity = 3,
                    UnitPrice = 10,
                    Product = new Product
                    {
                        Name = "Product 1",
                        ImgUrl = "imgs/prodImg.jpg",
                        Stock = 4
                    }
                },
                new CartItem
                {
                    Quantity = 8,
                    UnitPrice = 20,
                    Product = new Product
                    {
                        Name = "Product 2",
                        ImgUrl = "imgs/prodImg.jpg",
                        Stock = 10
                    }
                },
            }
        };
        var order = new Order()
        {
            Id = 1,
            Status = OrderStatus.PendingPayment,
            TotalAmount = 190,
            UserId = 1,
            OrderItems = new List<OrderItem>()
        };
        var createdOrder = new Order()
        {
            Id = order.Id,
            Status = OrderStatus.PendingPayment,
            TotalAmount = 190,
            UserId = 1,
            OrderItems = new List<OrderItem>()
        };
        var orderDetailDto = new OrderDetailDto()
        {
            Id = 1,
            Status = OrderStatus.PendingPayment,
            TotalAmount = 190,
            UserId = 1
        };
        _currentUserServiceMock.Setup(x => x.UserId).Returns(1);
        _cartRepositoryMock.Setup(x => x.GetByUserWithItemsAsync(1)).ReturnsAsync(cart);
        _orderRepositoryMock.Setup(x => x.Add(order));
        _orderRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(1, false)).ReturnsAsync(createdOrder);
        _mapperMock.Setup(x => x.Map<OrderDetailDto>(createdOrder)).Returns(orderDetailDto);

        var result = await _orderService.CreateOrderAsync();

        Assert.Empty(cart.CartItems);
        _orderRepositoryMock.Verify(x => x.Add(order), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    #endregion
}
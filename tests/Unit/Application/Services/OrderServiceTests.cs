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
    #endregion
}
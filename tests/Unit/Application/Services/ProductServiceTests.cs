using AutoMapper;
using EcommerceBackend.Application.Dto.Product;
using EcommerceBackend.Application.Exceptions;
using EcommerceBackend.Application.Interfaces;
using EcommerceBackend.Application.Interfaces.Services;
using EcommerceBackend.Application.Services;
using EcommerceBackend.Domain.Models;
using Moq;
using Xunit;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _productService = new ProductService
            (
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object
            );
    }

    #region Get By Id
    [Fact]
    public async Task GetById_ProductExists_ReturnProductDto()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg"
        };
        var productDto = new ProductDto
        {
            Id = 1,
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            CategoryName = "Category 1"
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductDto>(product)).Returns(productDto);

        var result = await _productService.GetById(1);

        Assert.NotNull(result);
        Assert.Equal(productDto.Id, result.Id);
        Assert.Equal(productDto.Name, result.Name);
    }

    [Fact]
    public async Task GetById_ProductDoesNotExist_ThrowNotFoundException()
    {
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetById(1));
    }
    #endregion

    #region Delete
    [Fact]
    public async Task DeleteAsync_ProductExists_DeleteSuccessfully()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg"
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        await _productService.DeleteAsync(1);
        _productRepositoryMock.Verify(x => x.Delete(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductDoesNotExist_ThrowNotFoundException()
    {
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetById(1));
    }
    #endregion

    #region Create
    [Fact]
    public async Task CreateAsync_ProductNameExists_ThrowAlreadyExistsException()
    {
        var product = new CreateProductDto
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            CategoryId = 1
        };
        var category = new Category
        {
            Id = 1,
            Name = "Category 1"
        };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(product.CategoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(x => x.IsNameExistsAsync(product.Name, category.Id)).ReturnsAsync(true);

        await Assert.ThrowsAsync<AlreadyExistsException>(() => _productService.CreateAsync(product));
    }

    [Fact]
    public async Task CreateAsync_CategoryIsNull_ThrowNotFoundException()
    {
        var product = new CreateProductDto
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            CategoryId = 1
        };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(product.CategoryId)).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _productService.CreateAsync(product));
    }
    #endregion
}
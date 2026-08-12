using AutoMapper;
using EcommerceBackend.Application.Common;
using EcommerceBackend.Application.Common.Query;
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

    [Fact]
    public async Task CreateAsync_ProductNameDoesNotExist_CreateSuccessfully()
    {
        var createProductDto = new CreateProductDto
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
        var product = new Product
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            CategoryId = 1
        };
        var productDto = new ProductDto
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            CategoryName = "Category 1"
        };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(createProductDto.CategoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(x => x.IsNameExistsAsync(createProductDto.Name, category.Id)).ReturnsAsync(false);
        _mapperMock.Setup(x => x.Map<Product>(createProductDto)).Returns(product);
        _mapperMock.Setup(x => x.Map<ProductDto>(product)).Returns(productDto);

        var result = await _productService.CreateAsync(createProductDto);

        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.ImgUrl, result.ImgUrl);
        Assert.Equal(category.Name, result.CategoryName);
        _productRepositoryMock.Verify(x => x.Add(product), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    #endregion

    #region Get All
    [Fact]
    public async Task GetAllAsync_ReturnPagedProductDtos()
    {
        var query = new ProductQueryParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var category1 = new Category
        {
            Name = "Category 1"
        };
        var category2 = new Category
        {
            Name = "Category 2"
        };
        var products = new List<Product>()
        {
            new Product
            {
                Name="Product 1",
                ImgUrl="imgs/prodImg1.jpg",
                Category=category1
            },
            new Product
            {
                Name="Product 2",
                ImgUrl="imgs/prodImg2.jpg",
                Category=category2
            }
        };
        var productDtos = new List<ProductDto>()
        {
            new ProductDto
            {
                Name="Product 1",
                ImgUrl="imgs/prodImg1.jpg",
                CategoryName=category1.Name
            },
            new ProductDto
            {
                Name="Product 2",
                ImgUrl="imgs/prodImg2.jpg",
                CategoryName=category2.Name
            }
        };
        var pagedResult = new PagedResult<Product>
        (
            products,
            2,
            1,
            10
        );
        _productRepositoryMock.Setup(x => x.GetAllAsync(query)).ReturnsAsync(pagedResult);
        _mapperMock.Setup(x => x.Map<List<ProductDto>>(pagedResult.Items)).Returns(productDtos);

        var result = await _productService.GetAllAsync(query);

        Assert.Equal(productDtos, result.Items);
        Assert.Equal(query.PageNumber, result.PageNumber);
        Assert.Equal(query.PageSize, result.PageSize);
        _productRepositoryMock.Verify(x => x.GetAllAsync(query), Times.Once);
    }
    #endregion

    #region Update
    [Fact]
    public async Task UpdateAsync_ProductDoesNotExist_ThrowNotFoundException()
    {
        var updateDto = new UpdateProductDto();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _productService.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_CategoryDoesNotExist_ThrowNotFoundException()
    {
        var updateDto = new UpdateProductDto
        {
            CategoryId = 1
        };
        var product = new Product
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg"
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(x => x.ExistsAsync(1)).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _productService.UpdateAsync(1, updateDto));
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(1), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductNameExistsInNewCategory_ThrowAlreadyExistsException()
    {
        var updateDto = new UpdateProductDto
        {
            CategoryId = 1
        };
        var product = new Product
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg"
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _productRepositoryMock.Setup(x => x.IsNameExistsAsync(product.Name, updateDto.CategoryId.Value, 1)).ReturnsAsync(true);

        await Assert.ThrowsAsync<AlreadyExistsException>(() => _productService.UpdateAsync(1, updateDto));
        _productRepositoryMock.Verify(x => x.IsNameExistsAsync(product.Name, updateDto.CategoryId.Value, 1), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NewProductNameExists_ThrowAlreadyExistsException()
    {
        var updateDto = new UpdateProductDto
        {
            Name = "New name"
        };
        var product = new Product
        {
            Name = "Product 1",
            ImgUrl = "imgs/prodImg.jpg",
            CategoryId = 1
        };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _productRepositoryMock.Setup(x => x.IsNameExistsAsync(updateDto.Name, product.CategoryId, 1)).ReturnsAsync(true);

        await Assert.ThrowsAsync<AlreadyExistsException>(() => _productService.UpdateAsync(1, updateDto));
    }
    #endregion
}
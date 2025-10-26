using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using TestAPIProject.Models;
using TestAPIProject.Repositories;
using TestAPIProject.Services;
using TestAPIProject.Dtos;
using AutoMapper;

namespace UnitTestProject;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsProducts()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "A", Price = 1 },
            new Product { Id = 2, Name = "B", Price = 2 }
        };
        mockUow.Setup(u => u.Products.GetAllAsync()).ReturnsAsync(products);

        var mockMapper = new Mock<IMapper>();
        var productDtos = products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price }).ToList();
        mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>())).Returns(productDtos);

        var service = new ProductService(mockUow.Object, mockMapper.Object);

        var result = await service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenFound()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var product = new Product { Id = 1, Name = "A", Price = 1 };
        mockUow.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<ProductDto?>(It.IsAny<Product>())).Returns(new ProductDto { Id = 1, Name = "A", Price = 1 });

        var service = new ProductService(mockUow.Object, mockMapper.Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAndCommits()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var createDto = new CreateProductDto { Name = "New", Price = 5 };

        var entity = new Product { Name = "New", Price = 5 };
        var createdEntity = new Product { Id = 5, Name = "New", Price = 5 };

        mockUow.Setup(u => u.Products.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdEntity);
        mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1).Verifiable();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductDto>())).Returns(entity);
        mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto { Id = 5, Name = "New", Price = 5 });

        var service = new ProductService(mockUow.Object, mockMapper.Object);

        var created = await service.CreateAsync(createDto);

        Assert.Equal(5, created.Id);
        mockUow.Verify();
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryAndCommits()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var updateDto = new TestAPIProject.Dtos.UpdateProductDto { Id = 1, Name = "Updated", Price = 10 };
        var entity = new Product { Id = 1, Name = "Updated", Price = 10 };

        mockUow.Setup(u => u.Products.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask).Verifiable();
        mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1).Verifiable();

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<Product>(It.IsAny<UpdateProductDto>())).Returns(entity);

        var service = new ProductService(mockUow.Object, mockMapper.Object);

        await service.UpdateAsync(updateDto);

        mockUow.Verify();
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryAndCommits()
    {
        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Products.DeleteAsync(1)).Returns(Task.CompletedTask).Verifiable();
        mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1).Verifiable();

        var mockMapper = new Mock<IMapper>();
        var service = new ProductService(mockUow.Object, mockMapper.Object);

        await service.DeleteAsync(1);

        mockUow.Verify();
    }
}

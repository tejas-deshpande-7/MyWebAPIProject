using Moq;
using TestAPIProject.Models;
using TestAPIProject.Repositories;
using TestAPIProject.Services;

namespace TestAPIProject.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsProducts()
    {
        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Products.GetAllAsync()).ReturnsAsync(new List<Product>
        {
            new Product { Id = 1, Name = "A", Price = 1 },
            new Product { Id = 2, Name = "B", Price = 2 }
        });

        var service = new ProductService(mockUow.Object);

        var result = await service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenFound()
    {
        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "A", Price = 1 });

        var service = new ProductService(mockUow.Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAndCommits()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var product = new Product { Name = "New", Price = 5 };
        mockUow.Setup(u => u.Products.CreateAsync(product)).ReturnsAsync(() => { product.Id = 5; return product; });
        mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1).Verifiable();

        var service = new ProductService(mockUow.Object);

        var created = await service.CreateAsync(product);

        Assert.Equal(5, created.Id);
        mockUow.Verify();
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryAndCommits()
    {
        var mockUow = new Mock<IUnitOfWork>();
        var product = new Product { Id = 1, Name = "Updated", Price = 10 };
        mockUow.Setup(u => u.Products.UpdateAsync(product)).Returns(Task.CompletedTask).Verifiable();
        mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1).Verifiable();

        var service = new ProductService(mockUow.Object);

        await service.UpdateAsync(product);

        mockUow.Verify();
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryAndCommits()
    {
        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.Products.DeleteAsync(1)).Returns(Task.CompletedTask).Verifiable();
        mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1).Verifiable();

        var service = new ProductService(mockUow.Object);

        await service.DeleteAsync(1);

        mockUow.Verify();
    }
}
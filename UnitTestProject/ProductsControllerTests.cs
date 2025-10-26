using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestAPIProject.Controllers;
using TestAPIProject.Dtos;
using TestAPIProject.Services;

namespace UnitTestProject;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        var mockService = new Mock<IProductService>();
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "A", Price = 1 },
            new ProductDto { Id = 2, Name = "B", Price = 2 }
        };
        mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        var controller = new ProductsController(mockService.Object);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenNull()
    {
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((ProductDto?)null);

        var controller = new ProductsController(mockService.Object);

        var result = await controller.Get(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenFound()
    {
        var mockService = new Mock<IProductService>();
        var product = new ProductDto { Id = 1, Name = "A", Price = 1 };
        mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        var controller = new ProductsController(mockService.Object);

        var result = await controller.Get(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ProductDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelInvalid()
    {
        var mockService = new Mock<IProductService>();
        var controller = new ProductsController(mockService.Object);
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Create(new CreateProductDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var mockService = new Mock<IProductService>();
        var createdProduct = new ProductDto { Id = 10, Name = "New", Price = 5 };
        mockService.Setup(s => s.CreateAsync(It.IsAny<CreateProductDto>())).ReturnsAsync(createdProduct);

        var controller = new ProductsController(mockService.Object);

        var result = await controller.Create(new CreateProductDto { Name = "New", Price = 5 });

        var createdAt = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ProductsController.Get), createdAt.ActionName);
        Assert.Equal(createdProduct, createdAt.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_OnIdMismatch()
    {
        var mockService = new Mock<IProductService>();
        var controller = new ProductsController(mockService.Object);

        var result = await controller.Update(1, new UpdateProductDto { Id = 2 });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask).Verifiable();

        var controller = new ProductsController(mockService.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        mockService.Verify();
    }
}

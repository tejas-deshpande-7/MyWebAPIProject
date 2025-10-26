using Xunit;
using Microsoft.EntityFrameworkCore;
using TestAPIProject.Data;
using TestAPIProject.Models;
using TestAPIProject.Repositories;
using System.Linq;

namespace UnitTestProject;

public class ProductRepositoryTests
{
    private DbContextOptions<AppDbContext> GetInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task CreateAndGetById_Workflow()
    {
        var options = GetInMemoryOptions("CreateAndGetById");
        using (var context = new AppDbContext(options))
        {
            var repo = new ProductRepository(context);
            var product = new Product { Name = "T", Price = 3 };
            var created = await repo.CreateAsync(product);
            // simulate UnitOfWork commit
            await context.SaveChangesAsync();

            Assert.True(created.Id > 0);

            var fetched = await repo.GetByIdAsync(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal("T", fetched!.Name);
        }
    }

    [Fact]
    public async Task Update_Works()
    {
        var options = GetInMemoryOptions("Update_Works");
        using (var context = new AppDbContext(options))
        {
            var repo = new ProductRepository(context);
            var product = new Product { Name = "Old", Price = 1 };
            var created = await repo.CreateAsync(product);
            await context.SaveChangesAsync();

            created.Name = "New";
            await repo.UpdateAsync(created);
            await context.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(created.Id);
            Assert.Equal("New", fetched!.Name);
        }
    }

    [Fact]
    public async Task Delete_Works()
    {
        var options = GetInMemoryOptions("Delete_Works");
        using (var context = new AppDbContext(options))
        {
            var repo = new ProductRepository(context);
            var product = new Product { Name = "ToDelete", Price = 1 };
            var created = await repo.CreateAsync(product);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(created.Id);
            await context.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(created.Id);
            Assert.Null(fetched);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsItems()
    {
        var options = GetInMemoryOptions("GetAll_ReturnsItems");
        using (var context = new AppDbContext(options))
        {
            context.Products.Add(new Product { Name = "P1", Price = 1 });
            context.Products.Add(new Product { Name = "P2", Price = 2 });
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);
            var list = await repo.GetAllAsync();
            Assert.Equal(2, list.Count());
        }
    }
}

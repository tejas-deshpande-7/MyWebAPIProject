using TestAPIProject.Dtos;
using TestAPIProject.Models;
using TestAPIProject.Repositories;

namespace TestAPIProject.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto product);
        Task UpdateAsync(UpdateProductDto product);
        Task DeleteAsync(int id);
    }
}
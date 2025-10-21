using TestAPIProject.Models;
using TestAPIProject.Repositories;

namespace TestAPIProject.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var created = await _unitOfWork.Products.CreateAsync(product);
            await _unitOfWork.CommitAsync();
            return created;
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return _unitOfWork.Products.GetAllAsync();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            return _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CommitAsync();
        }
    }
}
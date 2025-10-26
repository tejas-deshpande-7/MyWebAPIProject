using AutoMapper;
using TestAPIProject.Dtos;
using TestAPIProject.Models;
using TestAPIProject.Repositories;

namespace TestAPIProject.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto product)
        {
            var entity = _mapper.Map<Product>(product);
            var created = await _unitOfWork.Products.CreateAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProductDto>(created);
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return _mapper.Map<ProductDto?>(product);
        }

        public async Task UpdateAsync(UpdateProductDto product)
        {
            var entity = _mapper.Map<Product>(product);
            await _unitOfWork.Products.UpdateAsync(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}
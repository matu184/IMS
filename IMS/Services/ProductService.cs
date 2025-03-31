using IMS.Models;
using IMS.Models.Dtos;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<IEnumerable<Product>> GetLowStockAsync()
        {
            var all = await _repository.GetAllAsync();
            return all.Where(p => p.Quantity < p.MinimumQuantity).ToList();
        }


        public async Task<Product?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Product> AddAsync(Product product) => await _repository.AddAsync(product);

        public async Task<bool> UpdateAsync(int id, Product product)
        {
            if (id != product.Id) return false;

            await _repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(existing);
            return true;
        }

        public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, string? search = null)
        {
            var query = _repository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.Location)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

    }
}

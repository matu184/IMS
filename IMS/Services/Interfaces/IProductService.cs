using IMS.Models;
using IMS.Models.Dtos;

namespace IMS.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetLowStockAsync();

        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task<bool> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize, string? search = null);
    }
}

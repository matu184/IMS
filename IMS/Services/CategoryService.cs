using IMS.Models;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;

namespace IMS.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _repository.GetAllAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _repository.GetByIdAsync(id);

        public async Task<Category> AddAsync(Category category)
            => await _repository.AddAsync(category);
    }
}

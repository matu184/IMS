using IMS.Models;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;

namespace IMS.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _repository;

        public LocationService(ILocationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
            => await _repository.GetAllAsync();

        public async Task<Location?> GetByIdAsync(int id)
            => await _repository.GetByIdAsync(id);

        public async Task<Location> AddAsync(Location location)
            => await _repository.AddAsync(location);
    }
}

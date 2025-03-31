using IMS.Models;

namespace IMS.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task<Location> AddAsync(Location location);

    }
}

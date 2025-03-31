using IMS.Data;
using IMS.Models;
using IMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
            => await _context.Locations.ToListAsync();

        public async Task<Location?> GetByIdAsync(int id)
            => await _context.Locations.FindAsync(id);

        public async Task<Location> AddAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }
    }
}

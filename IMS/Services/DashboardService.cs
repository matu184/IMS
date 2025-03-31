using IMS.Data;
using IMS.Models.Dtos;
using IMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            return new DashboardOverviewDto
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalLocations = await _context.Locations.CountAsync(),
                LowStockCount = await _context.Products
                    .CountAsync(p => p.Quantity < p.MinimumQuantity)
            };
        }
    }
}

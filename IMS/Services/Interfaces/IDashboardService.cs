using IMS.Models.Dtos;

namespace IMS.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
    }
}

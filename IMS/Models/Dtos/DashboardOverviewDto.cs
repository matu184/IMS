namespace IMS.Models.Dtos
{
    public class DashboardOverviewDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalLocations { get; set; }
        public int LowStockCount { get; set; }
    }
}

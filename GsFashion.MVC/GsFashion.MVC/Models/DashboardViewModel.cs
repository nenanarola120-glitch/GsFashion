using GsFashion.Repository.Models.Report;

namespace GsFashion.MVC.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<DashboardMetricModel> Metrics { get; set; } = Enumerable.Empty<DashboardMetricModel>();
        public IEnumerable<MonthlyPaidAmountModel> MonthlyPaidAmounts { get; set; } = Enumerable.Empty<MonthlyPaidAmountModel>();
    }
}

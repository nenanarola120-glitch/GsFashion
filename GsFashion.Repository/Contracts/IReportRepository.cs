using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Report;

namespace GsFashion.Repository.Contracts
{
    public interface IReportRepository
    {
        Task<IEnumerable<InventoryItemModel>> GetAvailableCholisAsync(
            DateTime? rentalStartDate,
            DateTime? expectedReturnDate,
            string? status,
            int? itemId,
            string? searchingString);

        Task<IEnumerable<BookedCholiReportModel>> GetBookedCholiReportAsync(
            DateTime rentalStartDate,
            DateTime expectedReturnDate,
            int? itemId);

        Task<IEnumerable<DashboardMetricModel>> GetDashboardMetricsAsync();
        Task<IEnumerable<MonthlyPaidAmountModel>> GetMonthlyPaidAmountsAsync();
    }
}

using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Report;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public Task<IEnumerable<InventoryItemModel>> GetAvailableCholisAsync(
            DateTime? rentalStartDate,
            DateTime? expectedReturnDate,
            string? status,
            int? itemId,
            string? searchingString)
            => _reportRepository.GetAvailableCholisAsync(
                rentalStartDate, expectedReturnDate, status, itemId, searchingString);

        public Task<IEnumerable<BookedCholiReportModel>> GetBookedCholiReportAsync(
            DateTime rentalStartDate,
            DateTime expectedReturnDate,
            int? itemId)
            => _reportRepository.GetBookedCholiReportAsync(rentalStartDate, expectedReturnDate, itemId);

        public Task<IEnumerable<DashboardMetricModel>> GetDashboardMetricsAsync()
            => _reportRepository.GetDashboardMetricsAsync();

        public Task<IEnumerable<MonthlyPaidAmountModel>> GetMonthlyPaidAmountsAsync()
            => _reportRepository.GetMonthlyPaidAmountsAsync();
    }
}

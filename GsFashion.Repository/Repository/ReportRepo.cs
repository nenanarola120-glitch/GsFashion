using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Report;
using System.Data;

namespace GsFashion.Repository.Repository
{
    public class ReportRepo : IReportRepository
    {
        private const string ReportSp = "usp_manage_report_manager";
        private readonly IDbConnection _context;

        public ReportRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        public Task<IEnumerable<InventoryItemModel>> GetAvailableCholisAsync(DateTime? rentalStartDate,DateTime? expectedReturnDate,string? status,int? itemId,string? searchingString)
        {
            return _context.QueryAsync<InventoryItemModel>(
                ReportSp,
                new
                {
                    Type = SPEnum.GetAvailableForRental.ToString(),
                    rental_start_date = rentalStartDate?.Date,
                    expected_return_date = expectedReturnDate?.Date,
                    status,
                    item_id = itemId,
                    searching_string = searchingString
                },
                commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<BookedCholiReportModel>> GetBookedCholiReportAsync(
            DateTime rentalStartDate,
            DateTime expectedReturnDate,
            int? itemId)
        {
            return _context.QueryAsync<BookedCholiReportModel>(
                ReportSp,
                new
                {
                    Type = SPEnum.GetBookedCholiReport.ToString(),
                    rental_start_date = rentalStartDate.Date,
                    expected_return_date = expectedReturnDate.Date,
                    item_id = itemId
                },
                commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<DashboardMetricModel>> GetDashboardMetricsAsync()
            => _context.QueryAsync<DashboardMetricModel>(
                ReportSp,
                new { Type = "GetDashboardMetrics" },
                commandType: CommandType.StoredProcedure);

        public Task<IEnumerable<MonthlyPaidAmountModel>> GetMonthlyPaidAmountsAsync()
            => _context.QueryAsync<MonthlyPaidAmountModel>(
                ReportSp,
                new { Type = "GetMonthlyPaidAmounts" },
                commandType: CommandType.StoredProcedure);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GsFashion.MVC.Models;
using GsFashion.Service.Contracts;

namespace GsFashion.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IReportService _reportService;

        public DashboardController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            // ReportRepo uses one scoped database connection. Execute these reads
            // in sequence because Multiple Active Result Sets is not enabled.
            var metrics = await _reportService.GetDashboardMetricsAsync();
            var monthlyPaidAmounts = await _reportService.GetMonthlyPaidAmountsAsync();

            return View(new DashboardViewModel
            {
                Metrics = metrics,
                MonthlyPaidAmounts = monthlyPaidAmounts
            });
        }
    }
}

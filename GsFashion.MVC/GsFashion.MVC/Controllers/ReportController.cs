using GsFashion.MVC.Models;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly IReportService _reportService;

        public ReportController(
            IInventoryItemService inventoryItemService,
            IReportService reportService)
        {
            _inventoryItemService = inventoryItemService;
            _reportService = reportService;
        }

        #region Available Choli Report

        [HttpGet]
        public async Task<IActionResult> AvailableCholiReport(
            DateTime? fromDate,
            DateTime? toDate,
            string? status = "Available",
            int? itemId = null,
            string? searchTerm = null)
        {
            if (fromDate.HasValue != toDate.HasValue)
            {
                TempData["Error"] = "Select both from date and to date to check availability.";
                return RedirectToAction(nameof(AvailableCholiReport), new { status, itemId, searchTerm });
            }

            if (fromDate.HasValue && toDate!.Value.Date < fromDate.Value.Date)
            {
                TempData["Error"] = "To date cannot be before from date.";
                return RedirectToAction(nameof(AvailableCholiReport), new { fromDate, status, itemId, searchTerm });
            }

            var allItems = (await _inventoryItemService.GetAllAsync()).ToList();
            var items = (await _reportService.GetAvailableCholisAsync(
                fromDate,
                toDate,
                status,
                itemId,
                searchTerm)).ToList();

            return View(new AvailableCholiReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                ItemId = itemId,
                SearchTerm = searchTerm,
                Statuses = allItems
                    .Select(item => item.Status)
                    .Where(itemStatus => !string.IsNullOrWhiteSpace(itemStatus))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(itemStatus => itemStatus),
                CholiOptions = allItems.OrderBy(item => item.SkuCode),
                Items = items
            });
        }

        #endregion

        #region Booked Choli Report

        [HttpGet]
        public async Task<IActionResult> BookedCholiReport(DateTime? fromDate, DateTime? toDate, int? itemId)
        {
            var allItems = (await _inventoryItemService.GetAllAsync()).ToList();

            if (!fromDate.HasValue && !toDate.HasValue)
            {
                return View(new BookedCholiReportViewModel
                {
                    CholiOptions = allItems.OrderBy(item => item.SkuCode)
                });
            }

            if (!fromDate.HasValue || !toDate.HasValue)
            {
                TempData["Error"] = "Select both from date and to date to view booked cholis.";
                return RedirectToAction(nameof(BookedCholiReport), new { itemId });
            }

            if (toDate.Value.Date < fromDate.Value.Date)
            {
                TempData["Error"] = "To date cannot be before from date.";
                return RedirectToAction(nameof(BookedCholiReport), new { fromDate, itemId });
            }

            var items = await _reportService.GetBookedCholiReportAsync(fromDate.Value, toDate.Value, itemId);
            return View(new BookedCholiReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ItemId = itemId,
                CholiOptions = allItems.OrderBy(item => item.SkuCode),
                Items = items
            });
        }

        #endregion
    }
}

using GsFashion.MVC.Services;
using GsFashion.Repository.Models.Rental;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.Controllers
{
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly IInventoryItemService _inventoryItemService;
        private readonly IRentalPaymentService _rentalPaymentService;
        private readonly RentalBillPdfService _rentalBillPdfService;

        public RentalController(
            IRentalService rentalService,
            IInventoryItemService inventoryItemService,
            IRentalPaymentService rentalPaymentService,
            RentalBillPdfService rentalBillPdfService)
        {
            _rentalService = rentalService;
            _inventoryItemService = inventoryItemService;
            _rentalPaymentService = rentalPaymentService;
            _rentalBillPdfService = rentalBillPdfService;
        }

        #region Get All

        [HttpGet]
        public async Task<IActionResult> GetAllRentalCholiList()
        {
            var result = await _rentalService.GetAllAsync();
            return View(result);
        }

        #endregion

        #region Select Inventory - Get

        [HttpGet]
        public async Task<IActionResult> AddRentalCholi(string searchingString = null, string itemIds = null)
        {
            var inventoryItems = await _inventoryItemService.GetAllAsync(searchingString);

            var model = new RentalModel
            {
                BookingDate = DateTime.Now,
                RentalStartDate = DateTime.Today,
                Status = "Booked",
                ItemIds = itemIds,
                InventoryItemModels = inventoryItems
            };

            return View(model);
        }

        #endregion

        #region Continue Booking

        [HttpGet]
        public async Task<IActionResult> ContinueRentalBooking(string itemIds)
        {
            if (string.IsNullOrWhiteSpace(itemIds))
            {
                TempData["Error"] = "Please select at least one choli.";
                return RedirectToAction(nameof(AddRentalCholi));
            }

            var selectedIds = itemIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out int id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (!selectedIds.Any())
            {
                TempData["Error"] = "Please select at least one choli.";
                return RedirectToAction(nameof(AddRentalCholi));
            }

            // Get all inventory
            var inventoryItems = await _inventoryItemService.GetAllAsync();

            // Get only selected inventory
            var selectedItems = inventoryItems
                .Where(x => selectedIds.Contains(x.ItemId))
                .ToList();

            if (!selectedItems.Any())
            {
                TempData["Error"] = "Selected inventory items were not found.";
                return RedirectToAction(nameof(AddRentalCholi));
            }

            // Calculate initial rental amount
            decimal totalRent = selectedItems.Sum(x => x.BaseRentalPrice);

            // Calculate security deposit
            decimal securityDeposit = selectedItems.Sum(x => x.SecurityDeposit);

            var model = new RentalModel
            {
                BookingDate = DateTime.Now,
                RentalStartDate = DateTime.Today,
                Status = "Booked",
                ItemIds = string.Join(",", selectedIds),
                InventoryItemModels = selectedItems,
                TotalRentAmount = totalRent,
                SecurityDeposit = securityDeposit,
                GrandTotal = totalRent + securityDeposit
            };

            return View("AddRentalBooking", model);
        }

        #endregion

        #region Create Rental - Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRentalBooking(RentalModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ItemIds))
            {
                ModelState.AddModelError(nameof(model.ItemIds), "Please select at least one choli.");
            }

            if (string.IsNullOrWhiteSpace(model.CustomerFirstName))
            {
                ModelState.AddModelError(nameof(model.CustomerFirstName), "Customer first name is required.");
            }

            if (string.IsNullOrWhiteSpace(model.CustomerPhoneNumber))
            {
                ModelState.AddModelError(nameof(model.CustomerPhoneNumber), "Customer phone number is required.");
            }

            if (!model.RentalStartDate.HasValue)
            {
                ModelState.AddModelError(nameof(model.RentalStartDate), "Rental start date is required.");
            }

            if (!model.ExpectedReturnDate.HasValue)
            {
                ModelState.AddModelError(nameof(model.ExpectedReturnDate), "Expected return date is required.");
            }

            if (model.RentalStartDate.HasValue &&
                model.ExpectedReturnDate.HasValue &&
                model.ExpectedReturnDate.Value < model.RentalStartDate.Value)
            {
                ModelState.AddModelError(nameof(model.ExpectedReturnDate), "Expected return date cannot be before rental start date.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectedInventory(model);
                return View("AddRentalBooking", model);
            }

            // At booking, only the security deposit is collected. Rent is collected later.
            model.Status = "Booked";
            model.AmountPaid = model.SecurityDeposit;
            model.BalanceAmount = Math.Max(model.TotalRentAmount - model.Discount, 0);
            model.GrandTotal = model.TotalRentAmount + model.SecurityDeposit - model.Discount;

            var result = await _rentalService.InsertAsync(model);

            if (result.Status == 1)
            {
                TempData["Success"] = result.Message;

                if (result.Id.HasValue)
                {
                    return RedirectToAction(nameof(RentalBill), new { id = result.Id.Value });
                }

                return RedirectToAction(nameof(GetAllRentalCholiList));
            }

            TempData["Error"] = result.Message;

            await LoadSelectedInventory(model);

            return View("AddRentalBooking", model);
        }

        #endregion

        #region Rental Payments

        [HttpGet]
        public async Task<IActionResult> ManageRentalPayments(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental is null)
                return RedirectToAction(nameof(GetAllRentalCholiList));

            return View(new RentalPaymentPageModel
            {
                Rental = rental,
                Payments = await _rentalPaymentService.GetByRentalAsync(id)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordRentalPayment(int rentalId, string paymentType, DateTime? actualReturnDate, string? notes)
        {
            var result = await _rentalPaymentService.RecordAsync(rentalId, paymentType, actualReturnDate, notes);
            TempData[result.Status == 1 ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(ManageRentalPayments), new { id = rentalId });
        }

        #endregion

        #region Load Selected Inventory

        private async Task LoadSelectedInventory(RentalModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ItemIds))
            {
                model.InventoryItemModels = new List<GsFashion.Repository.Models.InventoryItem.InventoryItemModel>();
                return;
            }

            var selectedIds = model.ItemIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out int id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var inventoryItems = await _inventoryItemService.GetAllAsync();

            model.InventoryItemModels = inventoryItems
                .Where(x => selectedIds.Contains(x.ItemId))
                .ToList();
        }

        #endregion

        #region Edit Get

        [HttpGet]
        public async Task<IActionResult> EditRentalCholi(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);

            if (rental == null)
            {
                TempData["Error"] = "Rental booking not found.";
                return RedirectToAction(nameof(GetAllRentalCholiList));
            }

            return View(rental);
        }

        #endregion

        #region Edit Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRentalCholi(RentalModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _rentalService.UpdateAsync(model);

            if (result.Status == 1)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction(nameof(GetAllRentalCholiList));
            }

            TempData["Error"] = result.Message;

            return View(model);
        }

        #endregion

        #region Delete / Cancel

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRentalCholi(int id)
        {
            var result = await _rentalService.DeleteAsync(id);

            if (result.Status == 1)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(GetAllRentalCholiList));
        }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> DetailsRentalCholi(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);

            if (rental == null)
            {
                TempData["Error"] = "Rental booking not found.";
                return RedirectToAction(nameof(GetAllRentalCholiList));
            }

            return View(rental);
        }

        #endregion

        #region Rental Bill PDF

        [HttpGet]
        public async Task<IActionResult> RentalBill(int id)
        {
            var rental = await _rentalService.GetByIdAsync(id);

            if (rental == null)
            {
                TempData["Error"] = "Rental booking not found.";
                return RedirectToAction(nameof(GetAllRentalCholiList));
            }

            // Get all inventory items
            var inventoryItems = await _inventoryItemService.GetAllAsync();

            // Convert ItemIds "1,5,8" to a list of ints: 1, 5, 8
            var selectedIds = rental.ItemIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out int idValue) ? idValue : 0)
                .Where(x => x > 0)
                .ToList()
                ?? new List<int>();

            // Load selected inventory items
            rental.InventoryItemModels = inventoryItems
                .Where(x => selectedIds.Contains(x.ItemId))
                .ToList();

            // Generate PDF
            var pdf = _rentalBillPdfService.Generate(rental);

            var fileName = $"Rental-Bill-{rental.RentalId:D5}.pdf";

            return File(pdf, "application/pdf", fileName);
        }

        #endregion
    }
}

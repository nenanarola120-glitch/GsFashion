using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GsFashion.MVC.Controllers
{
    public class InventoryItemController : Controller
    {
        // matches the CHECK constraint on inventory_items.status
        private static readonly string[] StatusOptions = { "Available", "Rented", "InWash", "UnderRepair", "Retired" };
        private const string ImageRootPath = @"D:\GSFashion";
        private const string ImageUrlPrefix = "/gsfashion-images";

        private readonly IInventoryItemService _itemService;
        private readonly ICategoryService _categoryService;

        public InventoryItemController(IInventoryItemService itemService, ICategoryService categoryService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItem(string searchingString = null)
        {
            var items = await _itemService.GetAllAsync(searchingString);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> AddItem()
        {
            await PopulateDropdownsAsync();
            return View(new InventoryItemModel { Status = "Available" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(InventoryItemModel item, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(item);
            }
            var savedUrl = await SaveItemImageAsync(imageFile, item.SkuCode);
            if (savedUrl is not null)
                item.ImageUrl = savedUrl;


            var result = await _itemService.InsertAsync(item);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllItem));
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item is null)
                return NotFound();

            await PopulateDropdownsAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(InventoryItemModel item, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(item);
            }

            var existing = await _itemService.GetByIdAsync(item.ItemId);
            if (existing is null)
                return NotFound();

            if (imageFile is not null && imageFile.Length > 0)
            {
                // only touch the old file if a NEW one was actually uploaded —
                // otherwise a plain "edit price, don't touch the image" save would wipe the photo
                DeleteItemImageFile(existing.ImageUrl);

                var savedUrl = await SaveItemImageAsync(imageFile, item.SkuCode);
                item.ImageUrl = savedUrl ?? existing.ImageUrl;
            }
            else
            {
                item.ImageUrl = existing.ImageUrl;
            }

            var result = await _itemService.UpdateAsync(item);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllItem));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item is null)
                return NotFound();

            var result = await _itemService.DeleteAsync(id);
            if (result.Status != 0)
                DeleteItemImageFile(item.ImageUrl);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllItem));
        }

        private async Task PopulateDropdownsAsync()
        {
            ViewBag.Categories = (await _categoryService.GetCategoryDropDown()).ToList();
            ViewBag.StatusOptions = StatusOptions
                .Select(s => new SelectListItem { Value = s, Text = s })
                .ToList();
        }

        private async Task<string?> SaveItemImageAsync(IFormFile? imageFile, string folderKey)
        {
            if (imageFile is null || imageFile.Length == 0)
                return null;

            var folder = Path.Combine(ImageRootPath, folderKey);
            Directory.CreateDirectory(folder);

            var fileName = Path.GetFileName(imageFile.FileName); // strip any path segments for safety
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"{ImageUrlPrefix}/{folderKey}/{fileName}"; // this is what goes in <img src="...">
        }

        private void DeleteItemImageFile(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            if (!imageUrl.StartsWith(ImageUrlPrefix, StringComparison.OrdinalIgnoreCase))
                return; // not one of our managed images (e.g. an external URL) — don't touch it

            var relativePath = imageUrl.Substring(ImageUrlPrefix.Length).TrimStart('/');
            var physicalPath = Path.Combine(ImageRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(physicalPath))
            {
                try
                {
                    System.IO.File.Delete(physicalPath);
                }
                catch (IOException)
                {
                    // file locked/in use (e.g. still open elsewhere) — don't block the whole update over this
                }
            }
        }

        private void SetTempData(Response result)
        {
            if (result.Status == 0)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;
        }
    }
}
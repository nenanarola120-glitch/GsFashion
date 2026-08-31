using GsFashion.Repository.Models.Category;
using GsFashion.Repository.Models.Common;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GsFashion.MVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        #region Get All
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }
        #endregion

        #region Create - GET
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View(new CategoryModel());
        }
        #endregion

        #region Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CategoryModel category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var result = await _categoryService.InsertAsync(category);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllCategory));
        }
        #endregion

        #region Edit - GET
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound();

            return View(category);
        }
        #endregion

        #region Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryModel category)
        {
            if (!ModelState.IsValid)
                return View(category);

            var existing = await _categoryService.GetByIdAsync(category.CategoryId);
            if (existing is null)
                return NotFound();

            var result = await _categoryService.UpdateAsync(category);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllCategory));
        }
        #endregion

        #region Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound();

            var result = await _categoryService.DeleteAsync(id);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllCategory));
        }
        #endregion

        private void SetTempData(Response result)
        {
            if (result.Status == 0)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;
        }
    }
}
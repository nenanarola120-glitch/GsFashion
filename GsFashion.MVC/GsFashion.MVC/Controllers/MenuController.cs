using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GsFashion.MVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        #region Get All

        [HttpGet]
        public async Task<IActionResult> GetAllMenuList()
        {
            var menus = await _menuService.GetAllAsync();
            return View(menus);
        }

        #endregion

        #region Get By Id

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
            {
                return NotFound();
            }

            return Json(menu);
        }

        #endregion

        #region Create - GET

        [HttpGet]
        public async Task<IActionResult> AddMenu()
        {
            ViewBag.ParentMenus = await GetParentMenuOptionsAsync();

            return View(new MenuModel
            {
                IsActive = true
            });
        }

        #endregion

        #region Create - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenu(MenuModel menu)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentMenus = await GetParentMenuOptionsAsync();
                return View(menu);
            }

            var result = await _menuService.InsertAsync(menu);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllMenuList));
        }

        #endregion

        #region Edit - GET

        [HttpGet]
        public async Task<IActionResult> EditMenu(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
            {
                return NotFound();
            }

            ViewBag.ParentMenus = await GetParentMenuOptionsAsync(excludeMenuId: id);

            return View(menu);
        }

        #endregion

        #region Edit - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenu(MenuModel menu)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentMenus = await GetParentMenuOptionsAsync(excludeMenuId: menu.MenuId);
                return View(menu);
            }

            var result = await _menuService.UpdateAsync(menu);
            SetTempData(result);

            return RedirectToAction(nameof(GetAllMenuList));
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);

            if (menu == null)
            {
                return NotFound();
            }

            var result = await _menuService.DeleteAsync(id);
            SetTempData(result);

            TempData["SuccessMessage"] = "Menu deleted successfully.";

            return RedirectToAction(nameof(GetAllMenuList));
        }

        #endregion

        #region Menu Tree By Role

        [HttpGet]
        public async Task<IActionResult> GetMenuTree(int roleId)
        {
            var menus = await _menuService.GetMenuTreeForRoleAsync(roleId);
            return Json(menus);
        }

        #endregion

        private async Task<List<SelectListItem>> GetParentMenuSelectListAsync(int? excludeMenuId = null)
        {
            var allMenus = await _menuService.GetAllAsync(); // returns MenuModel with ParentMenuId

            return allMenus
                .Where(m => m.ParentMenuId == null)                                  // only top-level menus can be a parent
                .Where(m => excludeMenuId == null || m.MenuId != excludeMenuId.Value) // can't be its own parent
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new SelectListItem { Value = m.MenuId.ToString(), Text = m.MenuName })
                .ToList();
        }

        private async Task<List<MenuModel>> GetParentMenuOptionsAsync(int? excludeMenuId = null)
        {
            var menus = await _menuService.GetAllAsync();

            return menus
                .Where(x => x.ParentMenuId == null)                                   // only top-level menus can be a parent
                .Where(x => excludeMenuId == null || x.MenuId != excludeMenuId.Value)  // a menu can't be its own parent
                .OrderBy(x => x.DisplayOrder)
                .ToList();
        }

        private void SetTempData(Response result)
        {
            if (result.Status == 0)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
        }
    }
}
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllAsync()
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
            var menus = await _menuService.GetAllAsync();

            ViewBag.ParentMenus = menus
                .Where(x => x.ParentMenuId == null)
                .ToList();
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
                var menus = await _menuService.GetAllAsync();

                ViewBag.ParentMenus = menus
                    .Where(x => x.ParentMenuId == null)
                    .ToList();

                return View(menu);
            }


           var result = await _menuService.InsertAsync(menu);
            SetTempData(result);

            //TempData["SuccessMessage"] = "Menu added successfully.";

            return RedirectToAction(nameof(GetAllAsync));
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
            var menus = await _menuService.GetAllAsync();

            ViewBag.ParentMenus = menus
                .Where(x => x.ParentMenuId == null &&
                            x.MenuId != id)
                .ToList();

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
                var menus = await _menuService.GetAllAsync();

                ViewBag.ParentMenus = menus
                    .Where(x => x.ParentMenuId == null &&
                                x.MenuId != menu.MenuId)
                    .ToList();

                return View(menu);
            }

            var existingMenu = await _menuService.GetByIdAsync(menu.MenuId);

            if (existingMenu == null)
            {
                return NotFound();
            }

            var result = await _menuService.UpdateAsync(menu);
            SetTempData(result);

            TempData["SuccessMessage"] = "Menu updated successfully.";

            return RedirectToAction(nameof(GetAllAsync));
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

            var result =  await _menuService.DeleteAsync(id);
            SetTempData(result);

            TempData["SuccessMessage"] = "Menu deleted successfully.";

            return RedirectToAction(nameof(GetAllAsync));
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

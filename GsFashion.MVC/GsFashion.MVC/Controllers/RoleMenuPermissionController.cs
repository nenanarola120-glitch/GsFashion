using GsFashion.MVC.Models;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace GsFashion.MVC.Controllers
{
    // NOTE: protect this controller once your login approach (cookie or session) is finalized —
    // e.g. [Authorize] or the custom [SessionAuthorize] filter. Left open for now.
    public class RoleMenuPermissionController : Controller
    {
        private readonly IRoleMenuPermissionService _permissionService;
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;
        private readonly IAdminUserService _adminUserService;
        public RoleMenuPermissionController(
            IRoleMenuPermissionService permissionService,
            IRoleService roleService,
            IMenuService menuService, IAdminUserService adminUserService)
        {
            _permissionService = permissionService;
            _roleService = roleService;
            _adminUserService = adminUserService;
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            var permissions = await _permissionService.GetAllAsync();
            return View(permissions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new RoleMenuPermissionFormViewModel
            {
                Roles = (await _roleService.GetRoleDropDown()).ToList(),
                Menus = (await _menuService.GetMenuDropDown()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleMenuPermissionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var permission = new RoleMenuPermissionModel
            {
                RoleId = model.RoleId,
                MenuId = model.MenuId,
                CanView = model.CanView,
                CanAdd = model.CanAdd,
                CanEdit = model.CanEdit,
                CanDelete = model.CanDelete
            };

            var result = await _permissionService.CreateAsync(permission);

            if (result.Status == 0)
            {
                TempData["Error"] = result.Message;
                await PopulateDropdownsAsync(model);
                return View(model);
            }
            else
            {
                TempData["Success"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var permission = await _permissionService.GetByIdAsync(id);
            if (permission is null)
                return NotFound();

            var model = new RoleMenuPermissionFormViewModel
            {
                PermissionId = permission.PermissionId,
                RoleId = permission.RoleId,
                MenuId = permission.MenuId,
                CanView = permission.CanView,
                CanAdd = permission.CanAdd,
                CanEdit = permission.CanEdit,
                CanDelete = permission.CanDelete,
                Roles = (await _roleService.GetRoleDropDown()).ToList(),
                Menus = (await _menuService.GetMenuDropDown()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleMenuPermissionFormViewModel model)
        {
            if (id != model.PermissionId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var permission = new RoleMenuPermissionModel
            {
                PermissionId = model.PermissionId,
                RoleId = model.RoleId,
                MenuId = model.MenuId,
                CanView = model.CanView,
                CanAdd = model.CanAdd,
                CanEdit = model.CanEdit,
                CanDelete = model.CanDelete
            };

            var result = await _permissionService.UpdateAsync(permission);

            if (result.Status == 0)
            {
                TempData["Error"] = result.Message;
                await PopulateDropdownsAsync(model);
                return View(model);
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           var result = await _permissionService.DeleteAsync(id);
            if (result.Status == 0)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(RoleMenuPermissionFormViewModel model)
        {
            model.Roles = (await _roleService.GetRoleDropDown()).ToList();
            model.Menus = (await _menuService.GetMenuDropDown()).ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Matrix(int? userId)
        {
            var model = new PermissionMatrixPageViewModel
            {
                SelectedUserId = userId,
                Users = (await _adminUserService.GetUserDropDown()).ToList()
            };

            if (userId.HasValue)
            {
                var user = await _adminUserService.GetByIdAsync(userId.Value);
                if (user is null)
                    return NotFound();

                model.Username = user.Username;
                model.RoleId = user.RoleId;
                model.RoleName = user.RoleName;
                model.Rows = await _permissionService.GetPermissionMatrixAsync(user.RoleId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyMatrix(int userId, List<PermissionMatrixRow> rows)
        {
            var user = await _adminUserService.GetByIdAsync(userId);
            if (user is null)
                return NotFound();

            var result = await _permissionService.ApplyPermissionMatrixAsync(user.RoleId, rows ?? new List<PermissionMatrixRow>());

            if (result.Status == 0)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Matrix), new { userId });
        }

        //private async Task<List<DropDownResponse>> GetUserSelectListAsync()
        //{
        //    var users = await _adminUserRepository.GetAllAsync();
        //    return users
        //        .Where(u => u.IsActive)
        //        .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = u.UserId.ToString(), Text = u.Username })
        //        .ToList();
        //}
    }
}

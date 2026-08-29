using System.ComponentModel.DataAnnotations;
using GsFashion.Repository.Models.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GsFashion.MVC.Models
{
    public class RoleMenuPermissionFormViewModel
    {
        public int PermissionId { get; set; } // 0 when creating

        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please select a menu")]
        [Display(Name = "Menu")]
        public int MenuId { get; set; }

        [Display(Name = "Can View")]
        public bool CanView { get; set; } = true;

        [Display(Name = "Can Add")]
        public bool CanAdd { get; set; }

        [Display(Name = "Can Edit")]
        public bool CanEdit { get; set; }

        [Display(Name = "Can Delete")]
        public bool CanDelete { get; set; }

        public List<DropDownResponse> Roles { get; set; } = new();
        public List<DropDownResponse> Menus { get; set; } = new();

        public string? ErrorMessage { get; set; }
    }
}

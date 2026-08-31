using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Menu;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GsFashion.MVC.Models
{
    public class PermissionMatrixPageViewModel
    {
        public int? SelectedUserId { get; set; }
        public string? Username { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public List<DropDownResponse> Users { get; set; } = new();
        public List<PermissionMatrixRow> Rows { get; set; } = new();
    }
}

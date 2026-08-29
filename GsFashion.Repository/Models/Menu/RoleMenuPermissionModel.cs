namespace GsFashion.Repository.Models
{
    public class RoleMenuPermissionModel
    {
        public int PermissionId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }   // only populated by GetAllWithDetailsAsync (joined query)
        public int MenuId { get; set; }
        public string? MenuName { get; set; }   // only populated by GetAllWithDetailsAsync (joined query)
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace GsFashion.Repository.Models
{
    public class PermissionMatrixRow
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}

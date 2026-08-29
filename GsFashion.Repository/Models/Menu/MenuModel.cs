namespace GsFashion.Repository.Models
{
    // Column names match the result set of usp_get_menus_by_role exactly,
    // so Dapper can map it automatically with QueryAsync<MenuModel>.
    public class MenuModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public int? ParentMenuId { get; set; }
        public string? MenuUrl { get; set; }
        public string? IconClass { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } // only populated when loaded via usp_manage_menus, not usp_get_menus_by_role
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    // Nested version built in the Service layer for rendering the sidebar.
    public class MenuTreeModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? MenuUrl { get; set; }
        public string? IconClass { get; set; }
        public int DisplayOrder { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public List<MenuTreeModel> Children { get; set; } = new();
    }
}

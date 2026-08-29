namespace GsFashion.Repository.Models
{
    public class AdminUserModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }   // joined from roles in usp_manage_admin_users
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GsFashion.Repository.Models.Category
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GsFashion.Repository.Models.InventoryItem
{
    public class InventoryItemModel
    {
        public int ItemId { get; set; }

        [Required(ErrorMessage = "SKU code is required")]
        [Display(Name = "SKU Code")]
        public string SkuCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // populated only by GetAll/GetById (joined to categories); ignored on Insert/Update
        public string? CategoryName { get; set; }

        public string? Size { get; set; }
        public string? Color { get; set; }

        [Required(ErrorMessage = "Base rental price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base rental price must be greater than 0")]
        [Display(Name = "Base Rental Price")]
        public decimal BaseRentalPrice { get; set; }

        [Display(Name = "Security Deposit")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Purchase Cost")]
        public decimal PurchaseCost { get; set; }

        [Required]
        public string Status { get; set; } = "Available";

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
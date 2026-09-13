using GsFashion.Repository.Models.InventoryItem;

namespace GsFashion.MVC.Models
{
    public class AvailableCholiReportViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public int? ItemId { get; set; }
        public string? SearchTerm { get; set; }
        public IEnumerable<string> Statuses { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<InventoryItemModel> CholiOptions { get; set; } = Enumerable.Empty<InventoryItemModel>();
        public IEnumerable<InventoryItemModel> Items { get; set; } = Enumerable.Empty<InventoryItemModel>();
    }
}

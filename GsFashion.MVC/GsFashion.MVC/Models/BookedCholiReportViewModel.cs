using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Report;

namespace GsFashion.MVC.Models
{
    public class BookedCholiReportViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ItemId { get; set; }
        public IEnumerable<InventoryItemModel> CholiOptions { get; set; } = Enumerable.Empty<InventoryItemModel>();
        public IEnumerable<BookedCholiReportModel> Items { get; set; } = Enumerable.Empty<BookedCholiReportModel>();
    }
}

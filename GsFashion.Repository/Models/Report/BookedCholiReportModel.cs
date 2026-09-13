namespace GsFashion.Repository.Models.Report
{
    public class BookedCholiReportModel
    {
        public int ItemId { get; set; }
        public string SkuCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int BookingCount { get; set; }
    }
}

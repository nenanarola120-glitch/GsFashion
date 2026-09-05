namespace GsFashion.Repository.Models.InventoryItem
{
    public class InventoryItemDropDown
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal RentPrice { get; set; }
        public string Status { get; set; }
    }
}

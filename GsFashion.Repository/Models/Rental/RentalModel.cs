using GsFashion.Repository.Models.InventoryItem;

namespace GsFashion.Repository.Models.Rental
{
    public class RentalModel
    {
        public int RentalId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

        // Rental Details
        public DateTime? BookingDate { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public string? Status { get; set; }
        public string? ItemIds { get; set; }
        public IEnumerable<InventoryItemModel> InventoryItemModels { get; set; }
           = new List<InventoryItemModel>();

        public string? ConditionOut { get; set; }

        //Rental Charges
        public decimal TotalRentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public decimal LateFee { get; set; }
        public decimal DamageFee { get; set; }
        public decimal Discount { get; set; }
        // UI-only value. Discount is persisted as a currency amount.
        public decimal DiscountPercent { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }

        //Customer Details 
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public int? CustomerId { get; set; }
    }
}

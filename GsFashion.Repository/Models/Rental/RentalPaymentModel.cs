namespace GsFashion.Repository.Models.Rental
{
    public class RentalPaymentModel
    {
        public int RentalPaymentId { get; set; }
        public int RentalId { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }
}

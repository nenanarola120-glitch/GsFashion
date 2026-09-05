namespace GsFashion.Repository.Models.Rental
{
    public class RentalPaymentPageModel
    {
        public RentalModel Rental { get; set; } = new();
        public IEnumerable<RentalPaymentModel> Payments { get; set; } = new List<RentalPaymentModel>();
    }
}

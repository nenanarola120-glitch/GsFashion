using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Rental;

namespace GsFashion.Repository.Contracts
{
    public interface IRentalPaymentRepository
    {
        Task<IEnumerable<RentalPaymentModel>> GetByRentalAsync(int rentalId);
        Task<Response> RecordAsync(int rentalId, string paymentType, DateTime? actualReturnDate, string? notes);
    }
}

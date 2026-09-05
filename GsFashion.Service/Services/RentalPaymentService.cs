using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Rental;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Services
{
    public class RentalPaymentService : IRentalPaymentService
    {
        private readonly IRentalPaymentRepository _repository;
        public RentalPaymentService(IRentalPaymentRepository repository) => _repository = repository;
        public Task<IEnumerable<RentalPaymentModel>> GetByRentalAsync(int rentalId) => _repository.GetByRentalAsync(rentalId);
        public Task<Response> RecordAsync(int rentalId, string paymentType, DateTime? actualReturnDate, string? notes) => _repository.RecordAsync(rentalId, paymentType, actualReturnDate, notes);
    }
}

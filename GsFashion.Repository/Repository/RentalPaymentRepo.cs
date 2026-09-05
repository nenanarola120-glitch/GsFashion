using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Rental;
using System.Data;

namespace GsFashion.Repository.Repository
{
    public class RentalPaymentRepo : IRentalPaymentRepository
    {
        private readonly IDbConnection _context;
        public RentalPaymentRepo(DapperContext context) => _context = context.CreateConnection();

        public Task<IEnumerable<RentalPaymentModel>> GetByRentalAsync(int rentalId) =>
            _context.QueryAsync<RentalPaymentModel>("usp_manage_rental_payments", new { Type = "GetByRental", rental_id = rentalId }, commandType: CommandType.StoredProcedure);

        public Task<Response> RecordAsync(int rentalId, string paymentType, DateTime? actualReturnDate, string? notes) =>
            _context.QueryFirstAsync<Response>("usp_manage_rental_payments", new { Type = "Record", rental_id = rentalId, payment_type = paymentType, actual_return_date = actualReturnDate, notes }, commandType: CommandType.StoredProcedure);
    }
}

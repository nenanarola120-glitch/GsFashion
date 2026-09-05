using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Repository.Models.Rental;
using System.Data;

namespace GsFashion.Repository.Repository
{
    public class RentalRepo : IRentalRepository
    {
        private const string _rentalSp = "usp_manage_rentals";

        private readonly IDbConnection _context;

        public RentalRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        #region Get All

        public async Task<IEnumerable<RentalModel>> GetAllAsync()
        {
            var result = await _context.QueryAsync<RentalModel>(
                _rentalSp,
                new
                {
                    Type = SPEnum.GetAll.ToString()
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        #endregion

        #region Get By Id

        public async Task<RentalModel?> GetByIdAsync(int rentalId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<RentalModel>(
                _rentalSp,
                new
                {
                    Type = SPEnum.GetById.ToString(),
                    rental_id = rentalId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        #endregion

        #region Insert

        public async Task<Response> InsertAsync(RentalModel rental)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _rentalSp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    customer_id = rental.CustomerId,
                    rental_start_date = rental.RentalStartDate,
                    expected_return_date = rental.ExpectedReturnDate,
                    total_rent_amount = rental.TotalRentAmount,
                    security_deposit = rental.SecurityDeposit,
                    discount = rental.Discount,
                    grand_total = rental.GrandTotal,
                    amount_paid = rental.AmountPaid,
                    balance_amount = rental.BalanceAmount,
                    status = rental.Status,
                    notes = rental.Notes,
                    item_ids = rental.ItemIds,
                    condition_out = rental.ConditionOut,
                    first_name = rental.CustomerFirstName,
                    last_name = rental.CustomerLastName,
                    phone_number = rental.CustomerPhoneNumber,
                    email = rental.CustomerEmail,
                    address = rental.CustomerAddress
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        #endregion

        #region Update

        public async Task<Response> UpdateAsync(RentalModel rental)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _rentalSp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    rental_id = rental.RentalId,
                    customer_id = rental.CustomerId,
                    rental_start_date = rental.RentalStartDate,
                    expected_return_date = rental.ExpectedReturnDate,
                    total_rent_amount = rental.TotalRentAmount,
                    security_deposit = rental.SecurityDeposit,
                    discount = rental.Discount,
                    grand_total = rental.GrandTotal,
                    amount_paid = rental.AmountPaid,
                    balance_amount = rental.BalanceAmount,
                    status = rental.Status,
                    notes = rental.Notes,
                    item_ids = rental.ItemIds,
                    condition_out = rental.ConditionOut
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        #endregion

        #region Delete

        public async Task<Response> DeleteAsync(int rentalId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _rentalSp,
                new
                {
                    Type = SPEnum.Delete.ToString(),
                    rental_id = rentalId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        #endregion

        #region Customer Dropdown

        public async Task<IEnumerable<DropDownResponse>> GetCustomerDropDown()
        {
            var result = await _context.QueryAsync<DropDownResponse>(
                _rentalSp,
                new
                {
                    Type = SPEnum.CustomerDropDown.ToString()
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        #endregion
    }
}

using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Repository.Models.Rental;

namespace GsFashion.Repository.Contracts
{
    public interface IRentalRepository
    {
        Task<IEnumerable<RentalModel>> GetAllAsync(string? searchingString = null, string? status = null);

        Task<RentalModel?> GetByIdAsync(int rentalId);

        Task<Response> InsertAsync(RentalModel rental);

        Task<Response> UpdateAsync(RentalModel rental);

        Task<Response> DeleteAsync(int rentalId);

        Task<IEnumerable<DropDownResponse>> GetCustomerDropDown();

    }
}

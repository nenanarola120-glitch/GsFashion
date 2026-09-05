using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Repository.Models.Rental;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Service
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;

        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        #region Get All
        public async Task<IEnumerable<RentalModel>> GetAllAsync()
        {
            return await _rentalRepository.GetAllAsync();
        }
        #endregion

        #region Get By Id
        public async Task<RentalModel?> GetByIdAsync(int rentalId)
        {
            return await _rentalRepository.GetByIdAsync(rentalId);
        }
        #endregion

        #region Insert
        public async Task<Response> InsertAsync(RentalModel rental)
        {
            return await _rentalRepository.InsertAsync(rental);
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(RentalModel rental)
        {
            return await _rentalRepository.UpdateAsync(rental);
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int rentalId)
        {
            return await _rentalRepository.DeleteAsync(rentalId);
        }
        #endregion

        #region Customer DropDown
        public async Task<IEnumerable<DropDownResponse>> GetCustomerDropDown()
        {
            return await _rentalRepository.GetCustomerDropDown();
        }
        #endregion
    }
}
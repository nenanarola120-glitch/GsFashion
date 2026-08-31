using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;

        public AdminUserService(IAdminUserRepository adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
        }

        #region Get By Username

        public async Task<AdminUserModel?> GetByUsernameAsync(string username)
        {
            return await _adminUserRepository.GetByUsernameAsync(username);
        }

        #endregion

        #region Get By Id

        public async Task<AdminUserModel?> GetByIdAsync(int userId)
        {
            return await _adminUserRepository.GetByIdAsync(userId);
        }

        #endregion

        #region User Dropdown

        public async Task<IEnumerable<DropDownResponse>> GetUserDropDown()
        {
            return await _adminUserRepository.GetUserDropDown();
        }

        #endregion

        #region Get All

        public async Task<IEnumerable<AdminUserModel>> GetAllAsync()
        {
            return await _adminUserRepository.GetAllAsync();
        }

        #endregion

        #region Insert

        public async Task<Response> InsertAsync(AdminUserModel user)
        {
            return await _adminUserRepository.InsertAsync(user);
        }

        #endregion

        #region Update

        public async Task<Response> UpdateAsync(AdminUserModel user)
        {
            return await _adminUserRepository.UpdateAsync(user);
        }

        #endregion

        #region Delete

        public async Task<Response> DeleteAsync(int userId)
        {
            return await _adminUserRepository.DeleteAsync(userId);
        }

        #endregion
    }
}
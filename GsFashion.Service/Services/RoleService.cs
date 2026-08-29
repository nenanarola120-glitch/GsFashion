using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsFashion.Service.Services
{
    public class RoleService :IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository role)
        {
            _roleRepository = role;
        }
        public async Task<IEnumerable<RoleModel>> GetAllAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<RoleModel?> GetByIdAsync(int roleId)
        {
            return await _roleRepository.GetByIdAsync(roleId);
        }

        public async Task<IEnumerable<DropDownResponse>> GetRoleDropDown()
        {
            return await _roleRepository.GetRoleDropDown();
        }
        #region Insert
        public async Task<Response> InsertAsync(RoleModel role)
        {
            return await _roleRepository.InsertAsync(role);
        }
        #endregion

        #region Update
        public async Task<Response> UpdateAsync(RoleModel role)
        {
            return await _roleRepository.UpdateAsync(role);
        }
        #endregion

        #region Delete
        public async Task<Response> DeleteAsync(int roleId)
        {
            return await _roleRepository.DeleteAsync(roleId);
        }
        #endregion
    }
}

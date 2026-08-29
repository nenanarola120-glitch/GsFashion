using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models;
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

        public async Task<int> InsertAsync(RoleModel role)
        {
            return await _roleRepository.InsertAsync(role);
        }

        public async Task UpdateAsync(RoleModel role)
        {
            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteAsync(int roleId)
        {
            await _roleRepository.DeleteAsync(roleId);
        }
    }
}

using GsFashion.Repository.Models.Category;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<DropDownResponse>> GetCategoryDropDown();
        Task<IEnumerable<CategoryModel>> GetAllAsync();
        Task<CategoryModel?> GetByIdAsync(int categoryId);
        Task<Response> InsertAsync(CategoryModel category);
        Task<Response> UpdateAsync(CategoryModel category);
        Task<Response> DeleteAsync(int categoryId);
    }
}
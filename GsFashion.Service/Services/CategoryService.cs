using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models.Category;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<IEnumerable<DropDownResponse>> GetCategoryDropDown() => _categoryRepository.GetCategoryDropDown();
        public Task<IEnumerable<CategoryModel>> GetAllAsync() => _categoryRepository.GetAllAsync();
        public Task<CategoryModel?> GetByIdAsync(int categoryId) => _categoryRepository.GetByIdAsync(categoryId);
        public Task<Response> InsertAsync(CategoryModel category) => _categoryRepository.InsertAsync(category);
        public Task<Response> UpdateAsync(CategoryModel category) => _categoryRepository.UpdateAsync(category);
        public Task<Response> DeleteAsync(int categoryId) => _categoryRepository.DeleteAsync(categoryId);
    }
}
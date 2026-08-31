using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models.Category;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.Menu;
using System.Data;

namespace GsFashion.Repository.Repository
{
    public class CategoryRepo : ICategoryRepository
    {
        private const string _categorySp = "usp_manage_categories";
        private readonly IDbConnection _context;

        public CategoryRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        public async Task<IEnumerable<DropDownResponse>> GetCategoryDropDown()
        {
            var result = await _context.QueryAsync<DropDownResponse>(
                _categorySp,
                new { Type = SPEnum.DropDown.ToString() },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllAsync()
        {
            var result = await _context.QueryAsync<CategoryModel>(
                _categorySp,
                new { Type = SPEnum.GetAll.ToString() },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<CategoryModel?> GetByIdAsync(int categoryId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<CategoryModel>(
                _categorySp,
                new { Type = SPEnum.GetById.ToString(), category_id = categoryId },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        // usp_manage_categories Insert returns TWO result sets: new_category_id, then Message/Status.
        public async Task<Response> InsertAsync(CategoryModel category)
        {
            using var multi = await _context.QueryMultipleAsync(
                _categorySp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    name = category.Name,
                    description = category.Description
                },
                commandType: CommandType.StoredProcedure);

            var result = await multi.ReadFirstAsync<Response>();
            return result;
        }

        public async Task<Response> UpdateAsync(CategoryModel category)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _categorySp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    category_id = category.CategoryId,
                    name = category.Name,
                    description = category.Description
                },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> DeleteAsync(int categoryId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _categorySp,
                new { Type = SPEnum.Delete.ToString(), category_id = categoryId },
                commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
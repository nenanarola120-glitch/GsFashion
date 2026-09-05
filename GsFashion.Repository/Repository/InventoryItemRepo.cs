using Dapper;
using GsFashion.Repository.Contracts;
using GsFashion.Repository.Dapper;
using GsFashion.Repository.Enums;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Menu;
using System.Data;

namespace GsFashion.Repository.Repository
{
    public class InventoryItemRepo : IInventoryItemRepository
    {
        private const string _itemSp = "usp_manage_inventory_items";
        private readonly IDbConnection _context;

        public InventoryItemRepo(DapperContext context)
        {
            _context = context.CreateConnection();
        }

        public async Task<IEnumerable<DropDownResponse>> GetInventoryItemDropDown()
        {
            var result = await _context.QueryAsync<DropDownResponse>(
                _itemSp,
                new { Type = SPEnum.DropDown.ToString() },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<InventoryItemDropDown>> GetAvailableInventoryItems()
        {
            var result = await _context.QueryAsync<InventoryItemDropDown>(
                _itemSp,
                new
                {
                    Type = SPEnum.InventoryItemDropDown.ToString()
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<InventoryItemModel>> GetAllAsync(string? searchingString = null)
        {
            var result = await _context.QueryAsync<InventoryItemModel>(
                _itemSp,
                new { Type = SPEnum.GetAll.ToString(), searching_string = searchingString },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<InventoryItemModel>> GetAvailableForRentalAsync(DateTime rentalStartDate, DateTime expectedReturnDate, string? searchingString = null)
        {
            return await _context.QueryAsync<InventoryItemModel>(
                _itemSp,
                new
                {
                    Type = "GetAvailableForRental",
                    rental_start_date = rentalStartDate.Date,
                    expected_return_date = expectedReturnDate.Date,
                    searching_string = searchingString
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<InventoryItemModel?> GetByIdAsync(int itemId)
        {
            var result = await _context.QueryFirstOrDefaultAsync<InventoryItemModel>(
                _itemSp,
                new { Type = SPEnum.GetById.ToString(), item_id = itemId },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> InsertAsync(InventoryItemModel item)
        {
            using var multi = await _context.QueryMultipleAsync(
                _itemSp,
                new
                {
                    Type = SPEnum.Insert.ToString(),
                    sku_code = item.SkuCode,
                    name = item.Name,
                    category_id = item.CategoryId,
                    size = item.Size,
                    color = item.Color,
                    baserentalprice = item.BaseRentalPrice,
                    security_deposit = item.SecurityDeposit,
                    purchase_cost = item.PurchaseCost,
                    status = item.Status,
                    image_url = item.ImageUrl
                },
                commandType: CommandType.StoredProcedure);

            var result = await multi.ReadFirstAsync<Response>();
            return result;
        }

        public async Task<Response> UpdateAsync(InventoryItemModel item)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _itemSp,
                new
                {
                    Type = SPEnum.Update.ToString(),
                    item_id = item.ItemId,
                    sku_code = item.SkuCode,
                    name = item.Name,
                    category_id = item.CategoryId,
                    size = item.Size,
                    color = item.Color,
                    baserentalprice = item.BaseRentalPrice,
                    security_deposit = item.SecurityDeposit,
                    purchase_cost = item.PurchaseCost,
                    status = item.Status,
                    image_url = item.ImageUrl
                },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<Response> DeleteAsync(int itemId)
        {
            var result = await _context.QueryFirstAsync<Response>(
                _itemSp,
                new { Type = SPEnum.Delete.ToString(), item_id = itemId },
                commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}

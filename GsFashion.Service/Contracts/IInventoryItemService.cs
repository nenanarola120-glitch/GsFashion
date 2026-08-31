using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface IInventoryItemService
    {
        Task<IEnumerable<DropDownResponse>> GetInventoryItemDropDown();
        Task<IEnumerable<InventoryItemModel>> GetAllAsync();
        Task<InventoryItemModel?> GetByIdAsync(int itemId);
        Task<Response> InsertAsync(InventoryItemModel item);
        Task<Response> UpdateAsync(InventoryItemModel item);
        Task<Response> DeleteAsync(int itemId);
    }
}
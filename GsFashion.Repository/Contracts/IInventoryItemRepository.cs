using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Repository.Contracts
{
    public interface IInventoryItemRepository
    {
        Task<IEnumerable<DropDownResponse>> GetInventoryItemDropDown();
        Task<IEnumerable<InventoryItemDropDown>> GetAvailableInventoryItems();
        Task<IEnumerable<InventoryItemModel>> GetAllAsync(string? searchingString = null);
        Task<InventoryItemModel?> GetByIdAsync(int itemId);
        Task<Response> InsertAsync(InventoryItemModel item);
        Task<Response> UpdateAsync(InventoryItemModel item);
        Task<Response> DeleteAsync(int itemId);
    }
}

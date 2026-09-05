using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Menu;

namespace GsFashion.Service.Contracts
{
    public interface IInventoryItemService
    {
        Task<IEnumerable<DropDownResponse>> GetInventoryItemDropDown();
        Task<IEnumerable<InventoryItemDropDown>> GetAvailableInventoryItems();
        Task<IEnumerable<InventoryItemModel>> GetAllAsync(string? searchingString = null);
        Task<IEnumerable<InventoryItemModel>> GetAvailableForRentalAsync(DateTime rentalStartDate, DateTime expectedReturnDate, string? searchingString = null);
        Task<InventoryItemModel?> GetByIdAsync(int itemId);
        Task<Response> InsertAsync(InventoryItemModel item);
        Task<Response> UpdateAsync(InventoryItemModel item);
        Task<Response> DeleteAsync(int itemId);
    }
}

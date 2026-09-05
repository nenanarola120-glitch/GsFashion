using GsFashion.Repository.Contracts;
using GsFashion.Repository.Models.Common;
using GsFashion.Repository.Models.InventoryItem;
using GsFashion.Repository.Models.Menu;
using GsFashion.Repository.Repository;
using GsFashion.Service.Contracts;

namespace GsFashion.Service.Implementation
{
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IInventoryItemRepository _itemRepository;

        public InventoryItemService(IInventoryItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public Task<IEnumerable<DropDownResponse>> GetInventoryItemDropDown() => _itemRepository.GetInventoryItemDropDown();
        // RentalService
        public async Task<IEnumerable<InventoryItemDropDown>> GetAvailableInventoryItems()
        {
            return await _itemRepository.GetAvailableInventoryItems();
        }
        public Task<IEnumerable<InventoryItemModel>> GetAllAsync(string? searchingString = null) => _itemRepository.GetAllAsync(searchingString);
        public Task<IEnumerable<InventoryItemModel>> GetAvailableForRentalAsync(DateTime rentalStartDate, DateTime expectedReturnDate, string? searchingString = null) => _itemRepository.GetAvailableForRentalAsync(rentalStartDate, expectedReturnDate, searchingString);
        public Task<InventoryItemModel?> GetByIdAsync(int itemId) => _itemRepository.GetByIdAsync(itemId);
        public Task<Response> InsertAsync(InventoryItemModel item) => _itemRepository.InsertAsync(item);
        public Task<Response> UpdateAsync(InventoryItemModel item) => _itemRepository.UpdateAsync(item);
        public Task<Response> DeleteAsync(int itemId) => _itemRepository.DeleteAsync(itemId);
    }
}

using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryItemRepository;
        // private readonly CatalogClient catalogClient;
        private readonly IRepository<CatalogItem> catalogItemRepository;
        public ItemController(IRepository<InventoryItem> inventoryItemRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.inventoryItemRepository = inventoryItemRepository;
            this.catalogItemRepository = catalogItemRepository;
            // this.catalogClient = catalogClient;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            // var catalogItems = await catalogClient.GetCatalogItemsAsync();
            var inventoryItemEntities = await inventoryItemRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await catalogItemRepository.GetAllAsync(item => itemIds.Contains(item.Id));
            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(
                    catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await inventoryItemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryItemRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalogue.Contracts;
using Play.Catalogue.Service.Dtos;
using Play.Catalogue.Service.Entities;
using Play.Common;

namespace Play.Catalogue.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<Item> itemRepository;
        // private static int requestCounter = 0;
        private readonly IPublishEndpoint publishEndpoint;
        public ItemController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemRepository = itemRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            //Introduces temporal failures
            // requestCounter++;
            // Console.WriteLine($"Request {requestCounter}: Starting...");
            // if (requestCounter <= 2)
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }

            // if (requestCounter <= 4)
            // {
            //     Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error)");
            //     return StatusCode(500);
            // }
            var items = (await itemRepository.GetAllAsync())
                .Select(item => item.AsDto());

            // Console.WriteLine($"Request {requestCounter}: 200 (OK)");
            return Ok(items);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await itemRepository.CreateAsync(item);
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingEntity = await itemRepository.GetAsync(id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            existingEntity.Name = updateItemDto.Name;
            existingEntity.Description = updateItemDto.Description;
            existingEntity.Price = updateItemDto.Price;

            await itemRepository.UpdateAsync(existingEntity);
            await publishEndpoint.Publish(new CatalogItemUpdated(id, existingEntity.Name, existingEntity.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await itemRepository.DeleteAsync(item.Id);
            await publishEndpoint.Publish(new CatalogItemDeleted(id));
            return NoContent();
        }
    }
}
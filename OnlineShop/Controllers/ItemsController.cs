using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public ItemsController(IApplicationRepository<Category> categoryRepository, IApplicationRepository<Item> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] int categoryId)
        {
            return await _itemRepository.Context.Items.Where(i => i.CategoryId.Equals(categoryId)).ToListAsync();
        }

        [HttpGet("items/{id:int}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            return await _itemRepository.Context.Items.SingleAsync(i => i.Id.Equals(id));
        }

        [HttpPut("items/{id:int}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _itemRepository.SetModifiedState(item);

            try
            {
                await _itemRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _itemRepository.FindAsync(id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost("items")]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _itemRepository.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _itemRepository.Remove(item);
            await _itemRepository.SaveChangesAsync();

            return NoContent();
        }

        private readonly IApplicationRepository<Item> _itemRepository;
    }
}

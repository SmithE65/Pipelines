using LocalInventory.Api.Data;
using LocalInventory.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocalInventory.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController(InventoryDbContext context) : ControllerBase
{
    private readonly InventoryDbContext _context = context;

    // GET: api/Inventory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
    {
        return await _context.Inventory
            .Include(x => x.Product)
            .Include(x => x.Location)
                .ThenInclude(x => x.Parent)
            .ToListAsync();
    }

    // GET: api/Inventory/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
    {
        var inventoryItem = await _context.Inventory.FindAsync(id);

        return inventoryItem == null ? (ActionResult<InventoryItem>)NotFound() : (ActionResult<InventoryItem>)inventoryItem;
    }

    // PUT: api/Inventory/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInventoryItem(int id, UpdateInventoryItemDto inventoryItem)
    {
        if (id != inventoryItem.Id)
        {
            return BadRequest();
        }

        _context.Entry(inventoryItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InventoryItemExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Inventory
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<InventoryItem>> PostInventoryItem(InventoryItemDto inventoryItem)
    {
        var item = new InventoryItem
        {
            ProductId = inventoryItem.ProductId,
            LocationId = inventoryItem.LocationId,
            Quantity = inventoryItem.Quantity
        };

        _context.Inventory.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetInventoryItem", new { id = item.Id }, inventoryItem);
    }

    // DELETE: api/Inventory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        var inventoryItem = await _context.Inventory.FindAsync(id);
        if (inventoryItem == null)
        {
            return NotFound();
        }

        _context.Inventory.Remove(inventoryItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool InventoryItemExists(int id)
    {
        return _context.Inventory.Any(e => e.Id == id);
    }
}

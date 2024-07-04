using Microsoft.EntityFrameworkCore;

namespace LocalInventory.Api.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryItem> Inventory { get; set; }
}

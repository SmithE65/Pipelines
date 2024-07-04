namespace LocalInventory.Api.Data;

public class InventoryItem
{
    public int Id { get; set; }
    public required Product Product { get; set; }
    public required Location Location { get; set; }
    public int Quantity { get; set; }
}
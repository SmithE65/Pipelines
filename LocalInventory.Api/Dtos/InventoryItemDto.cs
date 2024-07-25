namespace LocalInventory.Api.Dtos;

public class InventoryItemDto
{
    public int ProductId { get; set; }
    public int LocationId { get; set; }
    public int Quantity { get; set; }
}


public class UpdateInventoryItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int LocationId { get; set; }
    public int Quantity { get; set; }
}

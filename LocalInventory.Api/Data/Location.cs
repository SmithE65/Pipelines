namespace LocalInventory.Api.Data;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Location? Parent { get; set; }
    public string Description { get; set; } = string.Empty;
}

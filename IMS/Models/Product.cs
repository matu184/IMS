using IMS.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public int MinimumQuantity { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public decimal Price { get; set; } // 🆕 Preisfeld ergänzt!
}

namespace Core.Entities;

public class ProductVariant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Weight { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}


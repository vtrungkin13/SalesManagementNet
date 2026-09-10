namespace Core.Entities;

public class Warehouse { public Guid Id { get; set; } public Guid TenantId { get; set; } public string Code { get; set; } = null!; public string Name { get; set; } = null!; public string? Address { get; set; } public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>(); public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>(); }



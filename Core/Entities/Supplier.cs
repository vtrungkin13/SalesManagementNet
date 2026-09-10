namespace Core.Entities;

public class Supplier { public Guid Id { get; set; } public Guid TenantId { get; set; } public string Code { get; set; } = null!; public string Name { get; set; } = null!; public string? Phone { get; set; } public string? Email { get; set; } public string? Address { get; set; } public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>(); }



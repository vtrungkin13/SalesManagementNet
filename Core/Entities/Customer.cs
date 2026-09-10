namespace Core.Entities;

public class Customer { public Guid Id { get; set; } public Guid TenantId { get; set; } public string Code { get; set; } = null!; public string Name { get; set; } = null!; public string? Phone { get; set; } public string? Email { get; set; } public string? Address { get; set; } public bool IsActive { get; set; } = true; public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>(); }



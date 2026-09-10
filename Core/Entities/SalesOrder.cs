namespace Core.Entities;

public class SalesOrder { public Guid Id { get; set; } public Guid TenantId { get; set; } public Guid CustomerId { get; set; } public Guid WarehouseId { get; set; } public string OrderNumber { get; set; } = null!; public string Status { get; set; } = "DRAFT"; public DateTime OrderDate { get; set; } public decimal Subtotal { get; set; } public decimal Discount { get; set; } public decimal Tax { get; set; } public decimal Total { get; set; } public string? Note { get; set; } public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public Customer Customer { get; set; } = null!; public Warehouse Warehouse { get; set; } = null!; public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>(); }



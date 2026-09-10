namespace Core.Entities;

public class Inventory { public Guid Id { get; set; } public Guid TenantId { get; set; } public Guid WarehouseId { get; set; } public Guid ProductVariantId { get; set; } public decimal Quantity { get; set; } public decimal ReservedQuantity { get; set; } public byte[] RowVersion { get; set; } = Array.Empty<byte>(); public DateTime UpdatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public Warehouse Warehouse { get; set; } = null!; public ProductVariant ProductVariant { get; set; } = null!; public decimal AvailableQuantity => Quantity - ReservedQuantity; }



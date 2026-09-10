using Core.Enums;
namespace Core.Entities;

public class InventoryTransaction { public Guid Id { get; set; } public Guid TenantId { get; set; } public Guid WarehouseId { get; set; } public Guid ProductVariantId { get; set; } public TransactionType Type { get; set; } public decimal Quantity { get; set; } public decimal BalanceAfter { get; set; } public string? ReferenceType { get; set; } public Guid? ReferenceId { get; set; } public string? Note { get; set; } public DateTime CreatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public Warehouse Warehouse { get; set; } = null!; public ProductVariant ProductVariant { get; set; } = null!; }



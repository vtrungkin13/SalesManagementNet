using Core.Enums;
namespace Core.Entities;

public class PurchaseOrder { public Guid Id { get; set; } public Guid TenantId { get; set; } public Guid SupplierId { get; set; } public Guid WarehouseId { get; set; } public string OrderNumber { get; set; } = null!; public PurchaseStatus Status { get; set; } = PurchaseStatus.PENDING; public DateTime OrderDate { get; set; } public decimal Subtotal { get; set; } public decimal Discount { get; set; } public decimal Tax { get; set; } public decimal Total { get; set; } public string? Note { get; set; } public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public Supplier Supplier { get; set; } = null!; public Warehouse Warehouse { get; set; } = null!; public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>(); public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>(); }



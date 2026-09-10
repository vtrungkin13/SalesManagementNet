namespace Core.Entities;

public class GoodsReceipt { public Guid Id { get; set; } public Guid TenantId { get; set; } public Guid PurchaseOrderId { get; set; } public Guid WarehouseId { get; set; } public string ReceiptNumber { get; set; } = null!; public DateTime ReceiptDate { get; set; } public string? Note { get; set; } public DateTime CreatedAt { get; set; } public Tenant Tenant { get; set; } = null!; public PurchaseOrder PurchaseOrder { get; set; } = null!; public Warehouse Warehouse { get; set; } = null!; public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>(); }



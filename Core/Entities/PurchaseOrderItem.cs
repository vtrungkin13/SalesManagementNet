namespace Core.Entities;

public class PurchaseOrderItem { public Guid Id { get; set; } public Guid PurchaseOrderId { get; set; } public Guid ProductVariantId { get; set; } public decimal Quantity { get; set; } public decimal UnitPrice { get; set; } public decimal Discount { get; set; } public decimal Tax { get; set; } public decimal Total { get; set; } public PurchaseOrder PurchaseOrder { get; set; } = null!; public ProductVariant ProductVariant { get; set; } = null!; }



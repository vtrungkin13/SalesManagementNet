namespace Core.Entities;

public class GoodsReceiptItem { public Guid Id { get; set; } public Guid GoodsReceiptId { get; set; } public Guid ProductVariantId { get; set; } public decimal Quantity { get; set; } public decimal UnitCost { get; set; } public GoodsReceipt GoodsReceipt { get; set; } = null!; public ProductVariant ProductVariant { get; set; } = null!; }



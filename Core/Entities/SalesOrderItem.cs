namespace Core.Entities;

public class SalesOrderItem { public Guid Id { get; set; } public Guid SalesOrderId { get; set; } public Guid ProductVariantId { get; set; } public decimal Quantity { get; set; } public decimal UnitPrice { get; set; } public decimal Discount { get; set; } public decimal Tax { get; set; } public decimal Total { get; set; } public SalesOrder SalesOrder { get; set; } = null!; public ProductVariant ProductVariant { get; set; } = null!; }



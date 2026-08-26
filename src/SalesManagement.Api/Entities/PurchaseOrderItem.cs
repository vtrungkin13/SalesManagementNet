using System;

namespace SalesManagement.Api.Entities
{
    public class PurchaseOrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public double UnitCost { get; set; }

        public Guid VariantId { get; set; }
        public ProductVariant? Variant { get; set; }

        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
    }
}

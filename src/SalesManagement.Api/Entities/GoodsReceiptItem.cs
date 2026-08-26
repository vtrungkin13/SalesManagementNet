using System;

namespace SalesManagement.Api.Entities
{
    public class GoodsReceiptItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public Guid GoodsReceiptId { get; set; }
        public GoodsReceipt? GoodsReceipt { get; set; }

        public Guid VariantId { get; set; }
        public ProductVariant? Variant { get; set; }
    }
}
